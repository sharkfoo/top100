using System;
using System.Collections;
using System.IO;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using Top40Modify;
using System.Text.RegularExpressions;

namespace iTunesExport.Parser
{
    /// <summary>
    /// Handles the parsing duties for the iTunes XML library.
    /// </summary>
    public class LibraryParser
    {
        private string _originalMusicFolder = null;
        private string _musicFolder = null;
        private Hashtable _songs = null;
        private Hashtable _playlists = null;

        #region Constructor

        /// <summary>
        /// Creates a new instance of LibraryParser for the iTunes XML library provided.
        /// </summary>
        /// <param name="fileLocation">The iTunes XML library to be parsed by this instance
        /// of the LibraryParser.</param>
        public LibraryParser( string fileLocation )
        {
            _songs = new Hashtable();
            _playlists = new Hashtable();

            parseLibrary( fileLocation );
        }

        #endregion

        #region Properties

        /// <summary>
        /// Provides the path to the Music Folder used by the current iTunes XML library.
        /// </summary>
        public string MusicFolder
        {
            get { return _musicFolder; }
        }

        /// <summary>
        /// An array of Playlist references, representing the playlists found in the current
        /// iTunes XML library.
        /// </summary>
        public Playlist[] Playlists
        {
            get 
            {
                Playlist[] playlists = new Playlist[_playlists.Count];
                _playlists.Values.CopyTo( playlists, 0 );
                return playlists;
            }
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Provides the default library location used by iTunes, based on the current
        /// machine's preferred music folder location.
        /// </summary>
        /// <returns>A string containing the path the default iTunes XML library location.</returns>
        public static string GetDefaultLibraryLocation()
        {
            string mymusicDataPath =
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            mymusicDataPath = Path.Combine(mymusicDataPath, "iTunes");
            mymusicDataPath = Path.Combine(mymusicDataPath, "iTunes Music Library.xml");
            return mymusicDataPath;
        }

        #endregion

        #region Private Parse Methods

        private void parseLibrary( string fileLocation )
        {
            StreamReader stream = new StreamReader(fileLocation, System.Text.Encoding.GetEncoding("utf-8"));
            XmlTextReader xmlReader = new XmlTextReader(stream);
            xmlReader.XmlResolver = null;
            XPathDocument xPathDocument = new XPathDocument(xmlReader);
            XPathNavigator xPathNavigator = xPathDocument.CreateNavigator();

            XPathNodeIterator nodeIterator = xPathNavigator.Select( "/plist/dict" );
            nodeIterator.MoveNext();
            nodeIterator = nodeIterator.Current.SelectChildren( XPathNodeType.All );
            while( nodeIterator.MoveNext() )
            {
                if( nodeIterator.Current.Value.Equals( "Music Folder" ) )
                {
                    if( nodeIterator.MoveNext() )
                    {
                        /// Parse out the location of the music folder used by the active library.
                        _originalMusicFolder = nodeIterator.Current.Value;
                        _musicFolder = _originalMusicFolder.Replace( "file://localhost/", String.Empty );

                        /// Fix to check for UNC paths, which don't have a drive letter and need the additional
                        /// slash at the front. Thanks to Chris Jenkins for finding this one.
                        if (_musicFolder.StartsWith("/"))
                            _musicFolder = string.Format("/{0}", _musicFolder);

                        _musicFolder = HttpUtility.UrlDecode( _musicFolder );
                        _musicFolder = _musicFolder.Replace( '/', Path.DirectorySeparatorChar );
                        break;
                    }
                }
            }


            /// Can't move on if we don't know where the music is stored.
            if (_musicFolder == null)
                throw new Exception("Unable to parse Music Library element from iTunes Music Library");

            /// This query gets us down to the point in the library that contains individual track details.
            nodeIterator = xPathNavigator.Select( "/plist/dict/dict/dict" );
            while( nodeIterator.MoveNext() )
            {
                /// Parse the track details, wherein a Track reference will be added to _tracks.
                parseTrack( nodeIterator.Current.SelectChildren( XPathNodeType.All ) );
            }

            /// After tracks, we're looking at the playlists that are listed in the library.
            nodeIterator = xPathNavigator.Select ("/plist/dict/array/dict");
            while( nodeIterator.MoveNext() )
            {
                /// Parse the playlist details wherein a Playlist reference will be added to _playlists.
                parsePlaylist( nodeIterator.Current.SelectChildren( XPathNodeType.All ) );
            }
        }

        private void parseTrack( XPathNodeIterator nodeIterator )
        {
            Song s = new Song();
            Regex comment = new Regex("(?<year>^[0-9][0-9][0-9][0-9]), #(?<number>[01][0-9][0-9]).*");

            string currentValue;
            while( nodeIterator.MoveNext() )
            {
                currentValue = nodeIterator.Current.Value;
                if (currentValue.Equals("Apple Music"))
                {
                    if (nodeIterator.MoveNext())
                    {
                        s.AppleMusic = Boolean.Parse(nodeIterator.Current.Name);
                    }
                }
                else if( currentValue.Equals( "Track ID" ) )
                {
                    if( nodeIterator.MoveNext() )
                    {
                        s.TrackId = Int32.Parse(nodeIterator.Current.Value);
                    }
                }
                else if( currentValue.Equals( "Name" ) )
                {
                    if( nodeIterator.MoveNext() )
                    {
                        s.Title = nodeIterator.Current.Value;
                    }
                }
                else if( currentValue.Equals( "Artist" ) )
                {
                    if( nodeIterator.MoveNext() )
                    {
                        s.Artist = nodeIterator.Current.Value;
                    }
                }
                else if( currentValue.Equals( "Grouping" ) )
                {
                    if( nodeIterator.MoveNext() )
                    {
                        s.Grouping = nodeIterator.Current.Value;
                    }
                }
                else if( currentValue.Equals( "Comments" ) )
                {
                    if( nodeIterator.MoveNext() )
                    {
                        s.Comments = nodeIterator.Current.Value;
                        if (comment.IsMatch(s.Comments))
                        {
                            Match m = comment.Match(s.Comments);

                            if (m.Success)
                            {
                                if (m.Groups["year"] != null && !String.IsNullOrEmpty(m.Groups["year"].Value))
                                {
                                    s.Year = Int16.Parse(m.Groups["year"].Value);
                                }
                                if (m.Groups["number"] != null && !String.IsNullOrEmpty(m.Groups["number"].Value))
                                {
                                    s.Number = Int16.Parse(m.Groups["number"].Value);
                                }
                            }
                        }
                    }
                }
            }

            if (s.TrackId > 0 && s.Title != null && s.Artist != null)
            {
                _songs.Add(s.TrackId.ToString(), new Song(s.TrackId, s.Title, s.Artist, s.Grouping, s.Comments, s.Year, s.Number, s.AppleMusic));
            }
        }

        private void parsePlaylist( XPathNodeIterator nodeIterator )
        {
            string id = null;
            string name = null;
            bool folder = false;
            ArrayList songs = new ArrayList();

            string currentName;
            string currentValue;
            while( nodeIterator.MoveNext() )
            {
                currentName = nodeIterator.Current.Name;
                if( currentName.Equals( "key" ) )
                {
                    currentValue = nodeIterator.Current.Value;
                    if( currentValue.Equals( "Name" ) )
                    {
                        if( nodeIterator.MoveNext() )
                        {
                            name = nodeIterator.Current.Value;
                        }
                    }
                    else if( currentValue.Equals( "Playlist ID" ) )
                    {
                        if( nodeIterator.MoveNext() )
                        {
                            id = nodeIterator.Current.Value;
                        }
                    }
                    else if( currentValue.Equals( "Folder" ) )
                    {
                        if( nodeIterator.MoveNext() )
                        {
                            folder = Boolean.Parse( nodeIterator.Current.Name );
                        }
                    }
                }
                else if( currentName.Equals( "array" ) )
                {
                    XPathNodeIterator songIterator = nodeIterator.Current.Select( "dict/integer" );
                    while( songIterator.MoveNext() )
                    {
                        songs.Add( songIterator.Current.Value );
                    }
                }
            }

            if( id != null && name != null && songs.Count > 0 )
            {
                _playlists.Add( id, new Playlist( id, name, folder, getSongs( songs ) ) );
            }
        }

        /// <summary>
        /// Returns an array containing a subset of all tracks, based on the passed track IDs list.       
        /// </summary>
        /// <param name="trackIds">The list of track IDs to be returned.</param>
        /// <returns>An array of Track references. If none are found, an empty array is returned.</returns>
        private Song[] getSongs( IList songIds )
        {
            Song[] songs = new Song[songIds.Count];
            int index = 0;
            foreach( string id in songIds )
            {
                var song = (Song)_songs[id];
                if (song != null)
                {
                    songs[index++] = song;
                }
                else
                {
                    Console.WriteLine("ERROR:  Song is null: " + id);
                }
            }

            return songs;
        }

        #endregion

    }
}
