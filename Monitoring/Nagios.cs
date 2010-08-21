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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using helpmebot6.Threading;

#endregion

namespace helpmebot6.Monitoring
{
    /// <summary>
    /// Nagios monitoring service
    /// </summary>
    internal class MonitorService : IThreadedSystem
    {
        private readonly TcpListener _service;

        private bool _alive;

        private readonly Thread _monitorthread;

        private readonly string _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorService"/> class.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="message">The message.</param>
        public MonitorService(int port, string message)
        {
            this._monitorthread = new Thread(threadMethod);

            this._message = message;

            this._service = new TcpListener(IPAddress.Any, port);
            this.registerInstance();
            this._monitorthread.Start();
        }

        private void threadMethod()
        {
            try
            {
                this._alive = true;
                this._service.Start();

                while (this._alive)
                {
                    if (!this._service.Pending())
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    TcpClient client = this._service.AcceptTcpClient();

                    StreamWriter sw = new StreamWriter(client.GetStream());

                    sw.WriteLine(this._message);
                    sw.Flush();
                    client.Close();
                }
            }
            catch (ThreadAbortException ex)
            {
                EventHandler temp = threadFatalError;
                if (temp != null)
                {
                    temp(this, new EventArgs());
                }

                GlobalFunctions.errorLog(ex);
            }
            catch(ObjectDisposedException ex)
            {
                EventHandler temp = threadFatalError;
                if (temp != null)
                {
                    temp(this, new EventArgs());
                }
 
                GlobalFunctions.errorLog(ex);
            }
        }

        /// <summary>
        /// Stop all threads in this instance to allow for a clean shutdown.
        /// </summary>
        public void stop()
        {
            this._service.Stop();
            this._alive = false;
        }

        #region IThreadedSystem Members

        public void registerInstance()
        {
            ThreadList.instance().register(this);
        }

        public string[] getThreadStatus()
        {
            string[] status = {"NagiosMonitor thread: " + this._monitorthread.ThreadState};
            return status;
        }

        public event EventHandler threadFatalError;

        #endregion
    }
}