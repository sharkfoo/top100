using System;
using Top100Modify;

namespace iTunesExport.Parser
{
    /// <summary>
    /// Represents an individual playlist from the iTunes library.
    /// </summary>
    public class Playlist
    {
        private string _id = null;
        private string _name = null;
        private bool _folder = false;
        private Song[] _songs = null;
        private char[] illegalCharacters = System.IO.Path.GetInvalidFileNameChars();

        public Playlist( string id, string name, bool folder, Song[] songs )
        {
            _id = id;
            _name = name;
            _folder = folder;

            foreach (char illegalChar in illegalCharacters)
                _name = _name.Replace(illegalChar, '-');
            
            _songs = songs;
        }

        /// <summary>
        /// The unique ID for this playlist.
        /// </summary>
        public string Id
        {
            get { return _id; }
        }

        /// <summary>
        /// The display name for this playlist.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Indicates whether this playlist is based on a folder in iTunes.
        /// </summary>
        public bool Folder
        {
            get { return _folder; }
        }

        /// <summary>
        /// An array of the Tracks that appear within this playlist.
        /// </summary>
        public Song[] Songs
        {
            get { return _songs; }
        }
    }
}
