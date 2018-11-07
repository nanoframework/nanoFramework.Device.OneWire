//
// Copyright (c) 2018 The nanoFramework project contributors
// See LICENSE file in the project root for full license information.
//

using System.Collections;
using System.Runtime.CompilerServices;

/// <summary>
/// Represents a 1-Wire bus. The object provides methods and properties that an app can use to interact with the bus.
/// </summary>
public class OneWire
{
    // this is used as the lock object 
    // a lock is required because multiple threads can access the SerialDevice
    private object _syncLock = new object();


    // external One Wire functions from link layer owllu.c

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
    /// TRUE: when a 1-Wire device was found and it's Serial Number placed in the global SerialNum[portnum]
    /// FALSE: There are no devices on the 1-Wire Net.
    /// </returns>
    [MethodImpl(MethodImplOptions.InternalCall)]
    public extern int FindFirstDevice(bool performResetBeforeSearch, bool searchWithAlarmCommand);

    /// <summary>
    ///  The function does a general search. This function continues from the previous search state. 
    ///  The search state can be reset by using the 'FindFirstDevice' function.
    /// </summary>
    /// <param name="performResetBeforeSearch">TRUE perform reset before search, FALSE do not perform reset before search.</param>
    /// <param name="searchWithAlarmCommand">TRUE the find alarm command 0xEC is sent instead of the normal search command 0xF0.</param>
    /// <returns>
    /// TRUE: when a 1-Wire device was found and it's Serial Number placed in the global SerialNum[portnum]
    /// FALSE: when no new device was found. Either the last search was the last device or there are no devices on the 1-Wire Net.
    /// </returns>
    [MethodImpl(MethodImplOptions.InternalCall)]
    public extern int FindNextDevice(bool performResetBeforeSearch, bool searchWithAlarmCommand);

    /// <summary>
    /// The function either reads or sets the SerialNum buffer
    /// that is used in the search functions 'FindFirstDevice' and 'FindNextDevice'.
    /// </summary>
    /// <param name="serialNumber">buffer to that contains the serial number to set when read = FALSE
    /// and buffer to get the serial number when read = TRUE.</param>
    /// <param name="read">flag to indicate reading (1) or setting (0) the current serial number.</param>
    /// <returns>
    /// TRUE: serial number read or set
    /// FALSE: no serial number
    /// </returns>
    [MethodImpl(MethodImplOptions.InternalCall)]
    public extern int SerialNumber(byte[] serialNumber, bool read);

    /// <summary>
    /// Find all devices present in 1-Wire Net
    /// </summary>
    /// <returns>
    /// ArrayList with the serial numbers of all devices found.
    /// </returns>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public ArrayList FindAllDevices()
    {
        int rslt;

        lock (_syncLock)
        {
            ArrayList serialNumbers = new ArrayList();

            // find the first device (all devices not just alarming)
            rslt = FindFirstDevice(true, false);

            while (rslt != 0)
            {
                byte[] SNum = new byte[8];

                // retrieve the serial number just found
                SerialNumber(SNum, true);

                // save serial number
                serialNumbers.Add(SNum);

                // find the next device
                rslt = FindNextDevice(true, false);
            }

            return serialNumbers;
        }
    }
}
