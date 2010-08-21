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
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

#endregion

namespace helpmebot6.UdpListener
{
    /// <summary>
    /// Represents a UDP communication message
    /// </summary>
    [Serializable]
    public class UdpMessage : ISerializable
    {
        /// <summary>
        /// Gets the hash of the message.
        /// </summary>
        /// <value>The hash.</value>
        public string hash { get; private set; }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string message { get; private set; }


        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <value>The command.</value>
        public string command { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="key">The key.</param>
        public UdpMessage(string message, string key)
        {
            this.message = message;
            this.command = "Message";
            byte[] bm = Encoding.ASCII.GetBytes(command + message + key);
            byte[] bh = MD5.Create().ComputeHash(bm);
            this.hash = Encoding.ASCII.GetString(bh);
        }

        public UdpMessage(string message, string key, string command)
        {
            this.message = message;
            this.command = command;
            byte[] bm = Encoding.ASCII.GetBytes(command + message + key);
            byte[] bh = MD5.Create().ComputeHash(bm);
            this.hash = Encoding.ASCII.GetString(bh);
        }

        #region ISerializable Members

        public UdpMessage(SerializationInfo info, StreamingContext ctxt)
        {
            this.hash = info.GetString("hash");
            this.message = info.GetString("message");
            this.command = info.GetString("command");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("hash", this.hash);
            info.AddValue("message", this.message);
            info.AddValue("command", this.command);
        }

        #endregion
    }
}