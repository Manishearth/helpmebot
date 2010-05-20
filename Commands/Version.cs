﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace helpmebot6.Commands
{
    /// <summary>
    /// Returns the current version of the bot.
    /// </summary>
    class Version : GenericCommand
    {

        public string version
        {
            get
            {
                return "6.0-backup";
            }
        }

        protected override CommandResponseHandler execute( User source , string channel , string[ ] args )
        {
  
                return new CommandResponseHandler( version );
        }

    }
}
