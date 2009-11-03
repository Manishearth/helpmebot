﻿using System;
using System.Collections.Generic;
using System.Text;

namespace helpmebot6
{
    public class Linker
    {
        Dictionary<string , string> lastLink;

        private static Linker _singleton;

        protected Linker( )
        {
            lastLink = new Dictionary<string , string>( );
            Helpmebot6.irc.PrivmsgEvent += new IAL.PrivmsgEventHandler( irc_PrivmsgEvent );
            Helpmebot6.irc.NoticeEvent += new IAL.PrivmsgEventHandler( irc_PrivmsgEvent );
        }

        void irc_PrivmsgEvent( User source , string destination , string message )
        {
            ParseMessage( message, destination );
        }

        public static Linker Instance( )
        {
            if( _singleton == null )
                _singleton = new Linker( );
            return _singleton;
        }

        public void ParseMessage( string Message, string Channel )
        {
            string newLink = "";
            
            if( ( Message.Contains( "[[" ) && Message.Contains( "]]" ) ) )
            { 
                // [[newLink]]

                int startIndex = Message.IndexOf( "[[" );
                int endIndex = Message.IndexOf( "]]" , startIndex );

                if( endIndex != -1 )
                {



                    int nextStartIndex = Message.IndexOf( "[[" , startIndex + 2 , endIndex - startIndex );

                    while( nextStartIndex != -1 )
                    {
                        startIndex = nextStartIndex;
                        nextStartIndex = Message.IndexOf( "[[" , startIndex + 2 , endIndex - startIndex );
                    }

                    newLink = Message.Substring( startIndex + 2 , endIndex - startIndex - 2 );
                }                
            }
            if( ( Message.Contains( "{{" ) && Message.Contains( "}}" ) ) )
            {
                int startIndex = Message.IndexOf( "{{" );
                int endIndex = Message.IndexOf( "}}" , startIndex );
                if( endIndex != -1 )
                {
                    int nextStartIndex = Message.IndexOf( "{{" , startIndex + 2 , endIndex - startIndex );

                    while( nextStartIndex != -1 )
                    {
                        startIndex = nextStartIndex;
                        nextStartIndex = Message.IndexOf( "{{" , startIndex + 2 , endIndex - startIndex );
                    }

                    newLink = "Template:" + Message.Substring( startIndex + 2 , endIndex - startIndex - 2 );
                }
            }
            newLink = newLink.Trim( '[' );
            if( newLink != "" )
            {
                if( lastLink.ContainsKey( Channel ) )
                {
                    lastLink.Remove( Channel );
                }
                lastLink.Add( Channel , newLink );
            }
        }

        public string GetLink( string destination )
        {
            string link;
            bool success = lastLink.TryGetValue( destination , out link );
            if( success )
                return "http://enwp.org/" + link;
            else
                return "";
        }
    }
}