﻿// /****************************************************************************
//  *   This file is part of Helpmebot.                                        *
//  *                                                                          *
//  *   Helpmebot is free software: you can redistribute it and/or modify      *
//  *   it under the terms of the GNU General Public License as published by   *
//  *   the Free Software Foundation, either version 3 of the License, or      *
//  *   (at your option) any later version.                                    *
//  *                                                                          *
//  *   Helpmebot is distributed in the hope that it will be useful,           *
//  *   but WITHOUT ANY WARRANTY; without even the implied warranty of         *
//  *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
//  *   GNU General Public License for more details.                           *
//  *                                                                          *
//  *   You should have received a copy of the GNU General Public License      *
//  *   along with Helpmebot.  If not, see <http://www.gnu.org/licenses/>.     *
//  ****************************************************************************/
#region Usings

using System;
using System.Reflection;

#endregion

namespace helpmebot6.Commands
{
    /// <summary>
    /// Modifies the bot's access list
    /// </summary>
    internal class Access : GenericCommand
    {
        /// <summary>
        /// Access denied to command, decide what to do
        /// </summary>
        /// <param name="source">The source of the command.</param>
        /// <param name="channel">The channel the command was triggered in.</param>
        /// <param name="args">The arguments to the command.</param>
        /// <returns>
        /// A response to the command if access to the command was denied
        /// </returns>
        protected override CommandResponseHandler accessDenied(User source, string channel, string[] args)
        {
            CommandResponseHandler crh = new Myaccess().run(source, channel, args);
            return crh;
        }

        /// <summary>
        /// Actual command logic
        /// </summary>
        /// <param name="source">The user who triggered the command.</param>
        /// <param name="channel">The channel the command was triggered in.</param>
        /// <param name="args">The arguments to the command.</param>
        /// <returns></returns>
        protected override CommandResponseHandler execute(User source, string channel, string[] args)
        {

            CommandResponseHandler crh = new CommandResponseHandler();
            if (args.Length > 1)
            {
                switch (args[0].ToLower())
                {
                    case "add":
                        if (args.Length > 2)
                        {
                            User.UserRights aL = User.UserRights.Normal;

                            switch (args[2].ToLower())
                            {
                                case "developer":
                                    aL = User.UserRights.Developer;
                                    break;
                                case "superuser":
                                    aL = User.UserRights.Superuser;
                                    break;
                                case "advanced":
                                    aL = User.UserRights.Advanced;
                                    break;
                                case "semi-ignored":
                                    aL = User.UserRights.Semiignored;
                                    break;
                                case "ignored":
                                    aL = User.UserRights.Ignored;
                                    break;
                                case "normal":
                                    aL = User.UserRights.Normal;
                                    break;
                                default:
                                    break;
                            }

                            crh = addAccessEntry(User.newFromString(args[1]), aL);
                        }
                        break;
                    case "del":
                        crh = delAccessEntry(int.Parse(args[1]));
                        break;
                }
                // add <source> <level>

                // del <id>
            }
            return crh;
        }

        /// <summary>
        /// Adds the access entry.
        /// </summary>
        /// <param name="newEntry">The new entry.</param>
        /// <param name="accessLevel">The access level.</param>
        /// <returns></returns>
        [Obsolete("Use User class")]
        private static CommandResponseHandler addAccessEntry(User newEntry, User.UserRights accessLevel)
        {
            string[] messageParams = {newEntry.ToString(), accessLevel.ToString()};
            string message = new Message().get("addAccessEntry", messageParams);

            // "Adding access entry for " + newEntry.ToString( ) + " at level " + accessLevel.ToString( )"
            Logger.instance().addToLog("Adding access entry for " + newEntry + " at level " + accessLevel,
                                       Logger.LogTypes.Command);
            DAL.singleton().insert("user", "", newEntry.nickname, newEntry.username, newEntry.hostname,
                                   accessLevel.ToString());

            return new CommandResponseHandler(message);
        }

        /// <summary>
        /// Dels the access entry.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [Obsolete("Use the User class")]
        private static CommandResponseHandler delAccessEntry(int id)
        {
            string[] messageParams = {id.ToString()};
            string message = new Message().get("removeAccessEntry", messageParams);

            Logger.instance().addToLog("Removing access entry #" + id, Logger.LogTypes.Command);
            DAL.singleton().delete("user", 1, new DAL.WhereConds("user_id", id.ToString()));

            return new CommandResponseHandler(message);
        }
    }
}