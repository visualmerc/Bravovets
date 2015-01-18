﻿namespace BravoVets.DomainObject.Infrastructure
{
    using System;

    /// <summary>
    /// Custom exception for structuring the error handling between BravoVets' service layer and UI
    /// </summary>
    public class BravoVetsException : Exception
    {
        private Guid _errorId;

        private string _originator;

        private Exception _bvInnerException;

        private string _bvMessage;

        /// <summary>
        /// ID is generated by the original source of the error, to correlate an error with its source
        /// </summary>
        public Guid ErrorId
        {
            get
            {
                return this._errorId;
            }
            set
            {
                this._errorId = value;
            }
        }

        /// <summary>
        /// Name of the method that throws the error
        /// </summary>
        public string Originator
        {
            get
            {
                return this._originator;
            }
            set
            {
                this._originator = value;
            }
        }

        public string BvMessage
        {
            get
            {
                return this._bvMessage;
            }
            set
            {
                this._bvMessage = value;
            }
        }

        public Exception BvInnerException
        {
            get
            {
                return this._bvInnerException;
            }
            set
            {
                this._bvInnerException = value;
            }
        }
    }
}
