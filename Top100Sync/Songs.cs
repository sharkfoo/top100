﻿//
// © Copyright 2020 Kevin Pearson
//
using System;
using System.Collections.Generic;

namespace Top100Sync
{
    public class Songs
    {
        private List<Song> songList = new List<Song>();

        public Songs()
        {
            var timer = Top100Timer.Start("Parsing MySQL top40 db");
            /*
            using (var selectCommand = dbConnection.CreateCommand())
            {
                selectCommand.CommandText = "SELECT * FROM songs";
                using (var reader = selectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var song = new Song();
                        song.DbId = (int)reader["id"];
                        song.Artist = (string)reader["artist"];
                        song.Title = (string)reader["title"];
                        song.Year = (Int16)reader["year"];
                        song.Number = (Int16)reader["number"];
                        song.Own = (bool)reader["own"];
                        songList.Add(song);
                    }
                }
            }
            */
            timer.End();
        }

        public List<Song> List
        {
            get
            {
                return songList;
            }
        }
    }
}

