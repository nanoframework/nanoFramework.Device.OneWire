//
// Copyright (c) .NET Foundation and Contributors
// See LICENSE file in the project root for full license information.
//

using System;
using System.Collections;
using System.Runtime.CompilerServices;

namespace nanoFramework.Device.OneWire
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneWireMaster"/> class.
    /// Represents a 1-Wire bus master. The class provides methods and properties that an application can use to interact with the 1-Wire bus and connected devices.
    /// </summary>
    public class OneWireMaster : IDisposable
    {
        // flag to signal that an instance of the class has been created
        private static bool s_opened = false;

        // this is used as the lock object 
        // a lock is required because multiple threads can access the FindAllDevices method
        private readonly object _syncLock;

        // this is the backing field for the serial number on discovery or when performing a read/write operation
        // need to create it here to be used in native
        private byte[] _serialNumber;

        private bool _disposed;

        // external One Wire functions from link layer owllu.c

        /// <summary>
        /// Initializes a new instance of the <see cref="OneWireMaster"/> class.
        /// </summary>
        /// <exception cref="InvalidOperationException">If there is already another instance of the <see cref="OneWireMaster"/> class.</exception>
        public OneWireMaster()
        {
            if (!s_opened)
            {
                _disposed = false;

                NativeInit();

                // flag that we have this open and initialized
                s_opened = true;

                // create lock object
                _syncLock = new();

                // create array for serial number
                _serialNumber = new byte[8];
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Reset all of the devices on the 1-Wire Net and return the result.
        /// </summary>
        /// <returns>
        /// TRUE: presence pulse(s) detected, device(s) reset.
        /// FALSE: no presence pulses detected.
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool TouchReset();

        /// <summary>
        /// Send 1 bit of communication to the 1-Wire Net and return the result 1 bit read from the 1-Wire Net.  
        /// The parameter <paramref name="value"/> least significant bit is used and the least significant bit of the result is the return bit.
        /// </summary>
        /// <param name="value">The least significant bit is the bit to send.</param>
        /// <returns>
        /// A 0 or 1 read from <paramref name="value"/>.
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool TouchBit(bool value);

        /// <summary>
        /// Send 8 bits of communication to the 1-Wire Net and return the
        /// result 8 bits read from the 1-Wire Net. The <paramref name="value"/>
        /// least significant 8 bits are used and the least significant 8 bits
        /// of the result is the return byte.
        /// </summary>
        /// <param name="value">8 bits to send (least significant byte).</param>
        /// <returns>
        /// 8 bits read from  <paramref name="value"/>
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern byte TouchByte(byte value);

        /// <summary>
        /// Send 8 bits of communication to the 1-Wire Net and verify that the
        /// 8 bits read from the 1-Wire Net is the same (write operation).
        /// </summary>
        /// <param name="value">8 bits to send (least significant byte).</param>
        /// <returns>
        /// TRUE: bytes written and echo was the same
        /// FALSE: echo was not the same
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern byte WriteByte(byte value);

        /// <summary>
        /// Sends 8 bits of read communication to the 1-Wire Net.
        /// </summary>
        /// <returns>
        /// 8 bit read from 1-Wire Net.
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern byte ReadByte();

        /// <summary>
        /// Finds the first device on the 1-Wire Net
        /// </summary>
        /// <param name="performResetBeforeSearch">TRUE perform reset before search, FALSE do not perform reset before search.</param>
        /// <param name="searchWithAlarmCommand">TRUE the find alarm command 0xEC is sent instead of the normal search command 0xF0.</param>
        /// <returns>
        /// TRUE: when a 1-Wire device was found and it's Serial Number placed in <see cref="SerialNumber"/>.
        /// FALSE: There are no devices on the 1-Wire Net.
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool FindFirstDevice(bool performResetBeforeSearch, bool searchWithAlarmCommand);

        /// <summary>
        ///  The function does a general search. This function continues from the previous search state. 
        ///  The search state can be reset by using the 'FindFirstDevice' function.
        /// </summary>
        /// <param name="performResetBeforeSearch">TRUE perform reset before search, FALSE do not perform reset before search.</param>
        /// <param name="searchWithAlarmCommand">TRUE the find alarm command 0xEC is sent instead of the normal search command 0xF0.</param>
        /// <returns>
        /// TRUE: when a 1-Wire device was found and it's Serial Number placed in <see cref="SerialNumber"/>.
        /// FALSE: when no new device was found. Either the last search was the last device or there are no devices on the 1-Wire Net.
        /// </returns>
        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern bool FindNextDevice(bool performResetBeforeSearch, bool searchWithAlarmCommand);

        /// <summary>
        /// SerialNum buffer that is used in the search methods <see cref="FindFirstDevice(bool, bool)"/> and <see cref="FindNextDevice(bool, bool)"/>.
        /// </summary>
        /// <returns>
        /// </returns>
        public byte[] SerialNumber
        {
            get
            {
                return _serialNumber;
            }

            set
            {
                _serialNumber = value;
            }
        }

        /// <summary>
        /// Find all devices present in 1-Wire Net
        /// </summary>
        /// <returns>
        /// ArrayList with the serial numbers of all devices found.
        /// </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ArrayList FindAllDevices()
        {
            bool result;

            lock (_syncLock)
            {
                ArrayList serialNumbers = new ArrayList();

                // find the first device (all devices not just alarming)
                result = FindFirstDevice(true, false);

                while (result)
                {
                    // retrieve and save the serial number just found
                    byte[] tmp = new byte[8];
                    _serialNumber.CopyTo(tmp, 0);
                    serialNumbers.Add(tmp);

                    // find the next device
                    result = FindNextDevice(true, false);
                }


                foreach(byte[] device in serialNumbers)
                {
                    string serial = "";

                    foreach (byte b in device)
                    {
                        serial += b.ToString("X2");
                    }

                    Console.WriteLine($"{serial}");
                }

                return serialNumbers;
            }
        }

        #region Dispose

        /// <inheritdoc/>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                NativeDispose();

                _disposed = true;
                s_opened = false;
            }
        }

        /// <inheritdoc/>
        ~OneWireMaster()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Native methods

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeDispose();

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern void NativeInit();

        #endregion
    }
}
