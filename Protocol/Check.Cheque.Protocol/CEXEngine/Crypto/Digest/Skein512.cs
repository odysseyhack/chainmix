﻿#region Directives
using System;
using VTDev.Libraries.CEXEngine.CryptoException;
using VTDev.Libraries.CEXEngine.Crypto.Enumeration;
#endregion

#region License Information
// The MIT License (MIT)
// 
// Copyright (c) 2016 vtdev.com
// This file is part of the CEX Cryptographic library.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// 
// Principal Algorithms:
// The Skein Hash Function Family: <a href="http://www.skein-hash.info/sites/default/files/skein1.1.pdf">Skein V1.1</a>.
// Implementation Details:
// An implementation of the Skein digest. 
// Written by John Underhill, January 13, 2015
// contact: develop@vtdev.com
#endregion

namespace VTDev.Libraries.CEXEngine.Crypto.Digest
{
    /// <summary>
    /// Skein512: An implementation of the Skein digest with a 512 bit digest return size
    /// </summary> 
    /// 
    /// <example>
    /// <description>Example using an <c>IDigest</c> interface:</description>
    /// <code>
    /// using (IDigest hash = new Skein512())
    /// {
    ///     // compute a hash
    ///     byte[] Output = ComputeHash(Input);
    /// }
    /// </code>
    /// </example>
    /// 
    /// <seealso cref="VTDev.Libraries.CEXEngine.Crypto.Digest.IDigest"/>
    /// <seealso cref="VTDev.Libraries.CEXEngine.Crypto.Enumeration.Digests"/>
    /// 
    /// <remarks>
    /// <description>Implementation Notes:</description>
    /// <list type="bullet">
    /// <item><description>Block size is 64 bytes, (512 bits).</description></item>
    /// <item><description>Digest size is 64 bytes, (512 bits).</description></item>
    /// <item><description>The <see cref="ComputeHash(byte[])"/> method wraps the <see cref="BlockUpdate(byte[], int, int)"/> and DoFinal methods, and resets the internal state.</description>/></item>
    /// <item><description>The <see cref="DoFinal(byte[], int)"/> method does NOT reset the internal state; call <see cref="Reset()"/> to reinitialize.</description></item>
    /// </list> 
    /// 
    /// <description>Guiding Publications:</description>
    /// <list type="number">
    /// <item><description>Skein <a href="http://www.skein-hash.info/sites/default/files/skein1.1.pdf">The Skein digest</a>.</description></item>
    /// <item><description>The Skein Hash Function Family <a href="http://www.skein-hash.info/sites/default/files/skein1.1.pdf">Skein V1.1</a>.</description></item>
    /// <item><description>Skein <a href="http://www.skein-hash.info/sites/default/files/skein-proofs.pdf">Provable Security</a> Support for the Skein Hash Family.</description></item>
    /// <item><description>NIST <a href="http://nvlpubs.nist.gov/nistpubs/ir/2012/NIST.IR.7896.pdf">SHA3 Third-Round Report</a> of the SHA-3 Cryptographic Hash Algorithm Competition>.</description></item>
    /// </list>
    /// 
    /// <description>Code Base Guides:</description>
    /// <list type="table">
    /// <item><description>Adapted from the excellent project by Alberto Fajardo: <a href="http://code.google.com/p/skeinfish/">Skeinfish Release 0.50</a>.</description></item>
    /// </list> 
    /// </remarks>
    public sealed class Skein512 : IDigest
    {
        #region Constants
        private const string ALG_NAME = "Skein512";
        private const int BLOCK_SIZE = 64;
        private const int DIGEST_SIZE = 64;
        private const int STATE_SIZE = 512;
        private const int STATE_BYTES = STATE_SIZE / 8;
        private const int STATE_WORDS = STATE_SIZE / 64;
        private const int STATE_OUTPUT = (STATE_SIZE + 7) / 8;
        #endregion

        #region Fields
        private int m_bytesFilled; 
        private Threefish512 m_blockCipher;
        private ulong[] m_cipherInput;
        private ulong[] m_configString;
        private ulong[] m_configValue;
        private SkeinInitializationType m_initializationType;
        private byte[] m_inputBuffer;
        private bool m_isDisposed = false;
        private ulong[] m_digestState;
        private UbiTweak m_ubiParameters;
        #endregion

        #region Properties
        /// <summary>
        /// Get: The Digests internal blocksize in bytes
        /// </summary>
        public int BlockSize 
        {
            get { return BLOCK_SIZE; }
        }

        /// <summary>
        /// The post-chain configuration value
        /// </summary>
        [CLSCompliant(false)]
        public ulong[] ConfigValue
        {
            get { return m_configValue; }
            private set { m_configValue = value; }
        }

        /// <summary>
        /// The pre-chain configuration string
        /// </summary>
        [CLSCompliant(false)]
        public ulong[] ConfigString
        {
            get { return m_configString; }
            private set { m_configString = value; }
        }

        /// <summary>
        /// Get: Size of returned digest in bytes
        /// </summary>
        public int DigestSize 
        {
            get { return DIGEST_SIZE; }
        }

        /// <summary>
        /// Get: The digests type name
        /// </summary>
        public Digests Enumeral
        {
            get { return Digests.Skein512; }
        }

        /// <summary>
        /// The initialization type
        /// </summary>
        public SkeinInitializationType InitializationType
        {
            get { return m_initializationType; }
            private set { m_initializationType = value; }
        }

        /// <summary>
        /// Get: The Digest name
        /// </summary>
        public string Name 
        {
            get { return ALG_NAME; }
        }

        /// <summary>
        /// State size in bits
        /// </summary>
        public int StateSize
        {
            get { return STATE_SIZE; }
        }

        /// <summary>
        /// Ubi Tweak parameters
        /// </summary>
        public UbiTweak UbiParameters
        {
            get { return m_ubiParameters; }
            private set { m_ubiParameters = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the Skein hash instance
        /// </summary>
        /// 
        /// <param name="InitializationType">Digest initialization type <see cref="SkeinInitializationType"/></param>
        public Skein512(SkeinInitializationType InitializationType = SkeinInitializationType.Normal)
        {
            m_initializationType = InitializationType;
            m_blockCipher = new Threefish512();
            // allocate buffers
            m_inputBuffer = new byte[STATE_BYTES];
            m_cipherInput = new ulong[STATE_WORDS];
            m_digestState = new ulong[STATE_WORDS];
            // allocate tweak
            m_ubiParameters = new UbiTweak();
            // allocate config value
            m_configValue = new ulong[STATE_BYTES];
            // initialize and enerate the configuration string
            m_configString = new ulong[STATE_BYTES];
            m_configString[1] = (ulong)DigestSize * 8;

            SetSchema(83, 72, 65, 51); // "SHA3"
            SetVersion(1);
            GenerateConfiguration();
            Initialize(InitializationType);
        }

        /// <summary>
        /// Finalize objects
        /// </summary>
        ~Skein512()
        {
            Dispose(false);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Update the buffer
        /// </summary>
        /// 
        /// <param name="Input">Input data</param>
        /// <param name="InOffset">Offset within Input</param>
        /// <param name="Length">Amount of data to process in bytes</param>
        /// 
        /// <exception cref="CryptoHashException">Thrown if an invalid Input size is chosen</exception>
        public void BlockUpdate(byte[] Input, int InOffset, int Length)
        {
            if ((InOffset + Length) > Input.Length)
                throw new CryptoHashException("Skein512:BlockUpdate", "The Input buffer is too short!", new ArgumentOutOfRangeException());

            int bytesDone = 0;
            int offset = InOffset;

            // fill input buffer
            while (bytesDone < Length && offset < Input.Length)
            {
                // do a transform if the input buffer is filled
                if (m_bytesFilled == STATE_BYTES)
                {
                    // copy input buffer to cipher input buffer
                    for (int i = 0; i < STATE_WORDS; i++)
                        m_cipherInput[i] = BytesToUInt64(m_inputBuffer, i * 8);

                    // process the block
                    ProcessBlock(STATE_BYTES);
                    // clear first flag, which will be set by Initialize() if this is the first transform
                    m_ubiParameters.IsFirstBlock = false;
                    // reset buffer fill count
                    m_bytesFilled = 0;
                }

                m_inputBuffer[m_bytesFilled++] = Input[offset++];
                bytesDone++;
            }
        }

        /// <summary>
        /// Get the Hash value.
        /// <para>Note: <see cref="Reset()"/> is called post hash calculation.</para> 
        /// </summary>
        /// 
        /// <param name="Input">Input data</param>
        /// 
        /// <returns>Hash value</returns>
        public byte[] ComputeHash(byte[] Input)
        {
            byte[] hash = new byte[DigestSize];

            BlockUpdate(Input, 0, Input.Length);
            DoFinal(hash, 0);
            Reset();

            return hash;
        }

        /// <summary>
        /// <para>Do final processing and get the hash value. 
        /// Note: Digest is not reset after calling DoFinal. 
        /// <see cref="Reset()"/> must be called before a new hash can be generated.</para>
        /// </summary>
        /// 
        /// <param name="Output">The Hash value container</param>
        /// <param name="OutOffset">The starting offset within the Output array</param>
        /// 
        /// <returns>Size of Hash value</returns>
        /// 
        /// <exception cref="CryptoHashException">Thrown if Output array is too small</exception>
        public int DoFinal(byte[] Output, int OutOffset)
        {
            if (Output.Length - OutOffset < DigestSize)
                throw new CryptoHashException("Skein512:DoFinal", "The Output buffer is too short!", new ArgumentOutOfRangeException());

            // pad left over space in input buffer with zeros and copy to cipher input buffer
            for (int i = m_bytesFilled; i < m_inputBuffer.Length; i++)
                m_inputBuffer[i] = 0;
            for (int i = 0; i < STATE_WORDS; i++)
                m_cipherInput[i] = BytesToUInt64(m_inputBuffer, i * 8);

            // do final message block
            m_ubiParameters.IsFinalBlock = true;
            ProcessBlock(m_bytesFilled);
            // clear cipher input
            Array.Clear(m_cipherInput, 0, m_cipherInput.Length);
            // do output block counter mode output
            byte[] hash = new byte[STATE_OUTPUT];
            ulong[] oldState = new ulong[STATE_WORDS];
            // save old state
            Array.Copy(m_digestState, oldState, m_digestState.Length);

            for (int i = 0; i < STATE_OUTPUT; i += STATE_BYTES)
            {
                m_ubiParameters.StartNewBlockType(UbiType.Out);
                m_ubiParameters.IsFinalBlock = true;
                ProcessBlock(8);

                // output a chunk of the hash
                int outputSize = STATE_OUTPUT - i;
                if (outputSize > STATE_BYTES)
                    outputSize = STATE_BYTES;

                PutBytes(m_digestState, hash, i, outputSize);
                // restore old state
                Array.Copy(oldState, m_digestState, oldState.Length);
                // increment counter
                m_cipherInput[0]++;
            }

            Buffer.BlockCopy(hash, 0, Output, OutOffset, hash.Length);

            return hash.Length;
        }

        /// <summary>
        /// Used to re-initialize the digest state.
        /// <para>Creates the initial state with zeros instead of the configuration block, then initializes the hash. 
        /// This does not start a new UBI block type, and must be done manually.</para>
        /// </summary>
        /// 
        /// <param name="InitializationType">Initialization parameters</param>
        public void Initialize(SkeinInitializationType InitializationType)
        {
            this.InitializationType = InitializationType;

            switch (InitializationType)
            {
                case SkeinInitializationType.Normal:
                    {
                        // normal initialization
                        Initialize();
                        return;
                    }
                case SkeinInitializationType.ZeroedState:
                    {
                        // copy the configuration value to the state
                        for (int i = 0; i < m_digestState.Length; i++)
                            m_digestState[i] = 0;

                        break;
                    }
                case SkeinInitializationType.ChainedConfig:
                    {
                        // generate a chained configuration
                        GenerateConfiguration(m_digestState);
                        // continue initialization
                        Initialize();
                        return;
                    }
                case SkeinInitializationType.ChainedState:
                    // keep the state as it is and do nothing
                    break;
            }

            // reset bytes filled
            m_bytesFilled = 0;
        }

        /// <summary>
        /// Reset the internal state
        /// </summary>
        public void Reset()
        {
            Initialize(InitializationType);
        }

        /// <summary>
        /// Update the message digest with a single byte
        /// </summary>
        /// 
        /// <param name="Input">Input byte</param>
        public void Update(byte Input)
        {
            BlockUpdate(new byte[] { Input }, 0, 1);
        }
        #endregion

        #region SkeinConfig
        /// <remarks>
        /// Default generation function
        /// </remarks>
        private void GenerateConfiguration()
        {
            Threefish512 cipher = new Threefish512();
            UbiTweak tweak = new UbiTweak();

            // initialize the tweak value
            tweak.StartNewBlockType(UbiType.Config);
            tweak.IsFinalBlock = true;
            tweak.BitsProcessed = 32;

            cipher.SetTweak(tweak.Tweak);
            cipher.Encrypt(m_configString, m_configValue);

            m_configValue[0] ^= m_configString[0];
            m_configValue[1] ^= m_configString[1];
            m_configValue[2] ^= m_configString[2];
        }

        /// <summary>
        /// Generate a configuration using a state key
        /// </summary>
        /// 
        /// <param name="InitialState">Twofish Cipher key</param>
        [CLSCompliant(false)]
        public void GenerateConfiguration(ulong[] InitialState)
        {
            Threefish512 cipher = new Threefish512();
            UbiTweak tweak = new UbiTweak();

            // initialize the tweak value
            tweak.StartNewBlockType(UbiType.Config);
            tweak.IsFinalBlock = true;
            tweak.BitsProcessed = 32;

            cipher.SetKey(InitialState);
            cipher.SetTweak(tweak.Tweak);
            cipher.Encrypt(m_configString, m_configValue);

            m_configValue[0] ^= m_configString[0];
            m_configValue[1] ^= m_configString[1];
            m_configValue[2] ^= m_configString[2];
        }

        /// <summary>
        /// Set the Schema. Schema must be 4 bytes.
        /// </summary>
        /// 
        /// <param name="Schema">Schema Configuration string</param>
        /// 
        /// <exception cref="CryptoSymmetricException">Thrown if an invalid schema is used</exception>
        public void SetSchema(params byte[] Schema)
        {
            if (Schema.Length != 4)
                throw new CryptoHashException("Skein512:SetSchema", "Schema must be 4 bytes.", new Exception());

            ulong n = m_configString[0];

            // clear the schema bytes
            n &= ~(ulong)0xfffffffful;
            // set schema bytes
            n |= (ulong)Schema[3] << 24;
            n |= (ulong)Schema[2] << 16;
            n |= (ulong)Schema[1] << 8;
            n |= (ulong)Schema[0];

            m_configString[0] = n;
        }

        /// <summary>
        /// Set the version string. Version must be between 0 and 3, inclusive.
        /// </summary>
        /// 
        /// <param name="Version">Version string</param>
        /// 
        /// <exception cref="CryptoSymmetricException">Thrown if an invalid version is used</exception>
        public void SetVersion(int Version)
        {
            if (Version < 0 || Version > 3)
                throw new CryptoHashException("Skein512:SetVersion", "Version must be between 0 and 3, inclusive.", new Exception());

            m_configString[0] &= ~((ulong)0x03 << 32);
            m_configString[0] |= (ulong)Version << 32;
        }

        /// <summary>
        /// Set the tree leaf size
        /// </summary>
        /// 
        /// <param name="Size">Leaf size</param>
        public void SetTreeLeafSize(byte Size)
        {
            m_configString[2] &= ~(ulong)0xff;
            m_configString[2] |= (ulong)Size;
        }

        /// <summary>
        /// Set the tree fan out size
        /// </summary>
        /// 
        /// <param name="Size">Fan out size</param>
        public void SetTreeFanOutSize(byte Size)
        {
            m_configString[2] &= ~((ulong)0xff << 8);
            m_configString[2] |= (ulong)Size << 8;
        }

        /// <summary>
        /// Set the tree height. Tree height must be zero or greater than 1.
        /// </summary>
        /// 
        /// <param name="Height">Tree height</param>
        /// 
        /// <exception cref="CryptoSymmetricException">Thrown if an invalid tree height is used</exception>
        public void SetMaxTreeHeight(byte Height)
        {
            if (Height == 1)
                throw new CryptoHashException("Skein512:SetMaxTreeHeight", "Tree height must be zero or greater than 1.", new Exception());

            m_configString[2] &= ~((ulong)0xff << 16);
            m_configString[2] |= (ulong)Height << 16;
        }
        #endregion

        #region Private Methods
        private void Initialize()
        {
            // copy the configuration value to the state
            for (int i = 0; i < m_digestState.Length; i++)
                m_digestState[i] = m_configValue[i];

            // set up tweak for message block
            m_ubiParameters.StartNewBlockType(UbiType.Message);
            // reset bytes filled
            m_bytesFilled = 0;
        }

        private void ProcessBlock(int bytes)
        {
            // set the key to the current state
            m_blockCipher.SetKey(m_digestState);
            // update tweak
            m_ubiParameters.BitsProcessed += (long)bytes;
            m_blockCipher.SetTweak(m_ubiParameters.Tweak);
            // encrypt block
            m_blockCipher.Encrypt(m_cipherInput, m_digestState);

            // feed-forward input with state
            for (int i = 0; i < m_cipherInput.Length; i++)
                m_digestState[i] ^= m_cipherInput[i];
        }

        private static ulong BytesToUInt64(byte[] Input, int InOffset)
        {
            ulong n = Input[InOffset];
            n |= (ulong)Input[InOffset + 1] << 8;
            n |= (ulong)Input[InOffset + 2] << 16;
            n |= (ulong)Input[InOffset + 3] << 24;
            n |= (ulong)Input[InOffset + 4] << 32;
            n |= (ulong)Input[InOffset + 5] << 40;
            n |= (ulong)Input[InOffset + 6] << 48;
            n |= (ulong)Input[InOffset + 7] << 56;

            return n;
        }

        private static void PutBytes(ulong[] Input, byte[] Output, int Offset, int ByteCount)
        {
            int j = 0;
            for (int i = 0; i < ByteCount; i++)
            {
                Output[Offset + i] = (byte)((Input[i / 8] >> j) & 0xff);
                j = (j + 8) % 64;
            }
        }
        #endregion

        #region Threefish512
        private class Threefish512
        {
            #region Constants
            private const int CipherSize = 512;
            private const int CipherQwords = CipherSize / 64;
            private const int ExpandedKeySize = CipherQwords + 1;
            private const ulong KeyScheduleConst = 0x1BD11BDAA9FC1A22;
            private const int ExpandedTweakSize = 3;
            #endregion

            #region Fields
            private ulong[] _expandedKey;
            private ulong[] _expandedTweak;
            #endregion

            #region Constructor
            internal Threefish512()
            {
                // create the expanded key array
                _expandedTweak = new ulong[ExpandedTweakSize];
                _expandedKey = new ulong[ExpandedKeySize];
                _expandedKey[ExpandedKeySize - 1] = KeyScheduleConst;
            }
            #endregion

            #region Internal Methods
            internal void Clear()
            {
                if (_expandedKey != null)
                {
                    Array.Clear(_expandedKey, 0, _expandedKey.Length);
                    _expandedKey = null;
                }
                if (_expandedTweak != null)
                {
                    Array.Clear(_expandedTweak, 0, _expandedTweak.Length);
                    _expandedTweak = null;
                }
            }

            internal void Encrypt(ulong[] Input, ulong[] Output)
            {
                // cache the block, key, and tweak
                ulong B0 = Input[0]; 
                ulong B1 = Input[1];
                ulong B2 = Input[2]; 
                ulong B3 = Input[3];
                ulong B4 = Input[4];
                ulong B5 = Input[5];
                ulong B6 = Input[6]; 
                ulong B7 = Input[7];
                ulong K0 = _expandedKey[0]; 
                ulong K1 = _expandedKey[1];
                ulong K2 = _expandedKey[2];
                ulong K3 = _expandedKey[3];
                ulong K4 = _expandedKey[4];
                ulong K5 = _expandedKey[5];
                ulong K6 = _expandedKey[6]; 
                ulong K7 = _expandedKey[7];
                ulong K8 = _expandedKey[8];
                ulong T0 = _expandedTweak[0];
                ulong T1 = _expandedTweak[1];
                ulong T2 = _expandedTweak[2];

                Mix(ref B0, ref B1, 46, K0, K1);
                Mix(ref B2, ref B3, 36, K2, K3);
                Mix(ref B4, ref B5, 19, K4, K5 + T0);
                Mix(ref B6, ref B7, 37, K6 + T1, K7);
                Mix(ref B2, ref B1, 33);
                Mix(ref B4, ref B7, 27);
                Mix(ref B6, ref B5, 14);
                Mix(ref B0, ref B3, 42);
                Mix(ref B4, ref B1, 17);
                Mix(ref B6, ref B3, 49);
                Mix(ref B0, ref B5, 36);
                Mix(ref B2, ref B7, 39);
                Mix(ref B6, ref B1, 44);
                Mix(ref B0, ref B7, 9);
                Mix(ref B2, ref B5, 54);
                Mix(ref B4, ref B3, 56);
                Mix(ref B0, ref B1, 39, K1, K2);
                Mix(ref B2, ref B3, 30, K3, K4);
                Mix(ref B4, ref B5, 34, K5, K6 + T1);
                Mix(ref B6, ref B7, 24, K7 + T2, K8 + 1);
                Mix(ref B2, ref B1, 13);
                Mix(ref B4, ref B7, 50);
                Mix(ref B6, ref B5, 10);
                Mix(ref B0, ref B3, 17);
                Mix(ref B4, ref B1, 25);
                Mix(ref B6, ref B3, 29);
                Mix(ref B0, ref B5, 39);
                Mix(ref B2, ref B7, 43);
                Mix(ref B6, ref B1, 8);
                Mix(ref B0, ref B7, 35);
                Mix(ref B2, ref B5, 56);
                Mix(ref B4, ref B3, 22);
                Mix(ref B0, ref B1, 46, K2, K3);
                Mix(ref B2, ref B3, 36, K4, K5);
                Mix(ref B4, ref B5, 19, K6, K7 + T2);
                Mix(ref B6, ref B7, 37, K8 + T0, K0 + 2);
                Mix(ref B2, ref B1, 33);
                Mix(ref B4, ref B7, 27);
                Mix(ref B6, ref B5, 14);
                Mix(ref B0, ref B3, 42);
                Mix(ref B4, ref B1, 17);
                Mix(ref B6, ref B3, 49);
                Mix(ref B0, ref B5, 36);
                Mix(ref B2, ref B7, 39);
                Mix(ref B6, ref B1, 44);
                Mix(ref B0, ref B7, 9);
                Mix(ref B2, ref B5, 54);
                Mix(ref B4, ref B3, 56);
                Mix(ref B0, ref B1, 39, K3, K4);
                Mix(ref B2, ref B3, 30, K5, K6);
                Mix(ref B4, ref B5, 34, K7, K8 + T0);
                Mix(ref B6, ref B7, 24, K0 + T1, K1 + 3);
                Mix(ref B2, ref B1, 13);
                Mix(ref B4, ref B7, 50);
                Mix(ref B6, ref B5, 10);
                Mix(ref B0, ref B3, 17);
                Mix(ref B4, ref B1, 25);
                Mix(ref B6, ref B3, 29);
                Mix(ref B0, ref B5, 39);
                Mix(ref B2, ref B7, 43);
                Mix(ref B6, ref B1, 8);
                Mix(ref B0, ref B7, 35);
                Mix(ref B2, ref B5, 56);
                Mix(ref B4, ref B3, 22);
                Mix(ref B0, ref B1, 46, K4, K5);
                Mix(ref B2, ref B3, 36, K6, K7);
                Mix(ref B4, ref B5, 19, K8, K0 + T1);
                Mix(ref B6, ref B7, 37, K1 + T2, K2 + 4);
                Mix(ref B2, ref B1, 33);
                Mix(ref B4, ref B7, 27);
                Mix(ref B6, ref B5, 14);
                Mix(ref B0, ref B3, 42);
                Mix(ref B4, ref B1, 17);
                Mix(ref B6, ref B3, 49);
                Mix(ref B0, ref B5, 36);
                Mix(ref B2, ref B7, 39);
                Mix(ref B6, ref B1, 44);
                Mix(ref B0, ref B7, 9);
                Mix(ref B2, ref B5, 54);
                Mix(ref B4, ref B3, 56);
                Mix(ref B0, ref B1, 39, K5, K6);
                Mix(ref B2, ref B3, 30, K7, K8);
                Mix(ref B4, ref B5, 34, K0, K1 + T2);
                Mix(ref B6, ref B7, 24, K2 + T0, K3 + 5);
                Mix(ref B2, ref B1, 13);
                Mix(ref B4, ref B7, 50);
                Mix(ref B6, ref B5, 10);
                Mix(ref B0, ref B3, 17);
                Mix(ref B4, ref B1, 25);
                Mix(ref B6, ref B3, 29);
                Mix(ref B0, ref B5, 39);
                Mix(ref B2, ref B7, 43);
                Mix(ref B6, ref B1, 8);
                Mix(ref B0, ref B7, 35);
                Mix(ref B2, ref B5, 56);
                Mix(ref B4, ref B3, 22);
                Mix(ref B0, ref B1, 46, K6, K7);
                Mix(ref B2, ref B3, 36, K8, K0);
                Mix(ref B4, ref B5, 19, K1, K2 + T0);
                Mix(ref B6, ref B7, 37, K3 + T1, K4 + 6);
                Mix(ref B2, ref B1, 33);
                Mix(ref B4, ref B7, 27);
                Mix(ref B6, ref B5, 14);
                Mix(ref B0, ref B3, 42);
                Mix(ref B4, ref B1, 17);
                Mix(ref B6, ref B3, 49);
                Mix(ref B0, ref B5, 36);
                Mix(ref B2, ref B7, 39);
                Mix(ref B6, ref B1, 44);
                Mix(ref B0, ref B7, 9);
                Mix(ref B2, ref B5, 54);
                Mix(ref B4, ref B3, 56);
                Mix(ref B0, ref B1, 39, K7, K8);
                Mix(ref B2, ref B3, 30, K0, K1);
                Mix(ref B4, ref B5, 34, K2, K3 + T1);
                Mix(ref B6, ref B7, 24, K4 + T2, K5 + 7);
                Mix(ref B2, ref B1, 13);
                Mix(ref B4, ref B7, 50);
                Mix(ref B6, ref B5, 10);
                Mix(ref B0, ref B3, 17);
                Mix(ref B4, ref B1, 25);
                Mix(ref B6, ref B3, 29);
                Mix(ref B0, ref B5, 39);
                Mix(ref B2, ref B7, 43);
                Mix(ref B6, ref B1, 8);
                Mix(ref B0, ref B7, 35);
                Mix(ref B2, ref B5, 56);
                Mix(ref B4, ref B3, 22);
                Mix(ref B0, ref B1, 46, K8, K0);
                Mix(ref B2, ref B3, 36, K1, K2);
                Mix(ref B4, ref B5, 19, K3, K4 + T2);
                Mix(ref B6, ref B7, 37, K5 + T0, K6 + 8);
                Mix(ref B2, ref B1, 33);
                Mix(ref B4, ref B7, 27);
                Mix(ref B6, ref B5, 14);
                Mix(ref B0, ref B3, 42);
                Mix(ref B4, ref B1, 17);
                Mix(ref B6, ref B3, 49);
                Mix(ref B0, ref B5, 36);
                Mix(ref B2, ref B7, 39);
                Mix(ref B6, ref B1, 44);
                Mix(ref B0, ref B7, 9);
                Mix(ref B2, ref B5, 54);
                Mix(ref B4, ref B3, 56);
                Mix(ref B0, ref B1, 39, K0, K1);
                Mix(ref B2, ref B3, 30, K2, K3);
                Mix(ref B4, ref B5, 34, K4, K5 + T0);
                Mix(ref B6, ref B7, 24, K6 + T1, K7 + 9);
                Mix(ref B2, ref B1, 13);
                Mix(ref B4, ref B7, 50);
                Mix(ref B6, ref B5, 10);
                Mix(ref B0, ref B3, 17);
                Mix(ref B4, ref B1, 25);
                Mix(ref B6, ref B3, 29);
                Mix(ref B0, ref B5, 39);
                Mix(ref B2, ref B7, 43);
                Mix(ref B6, ref B1, 8);
                Mix(ref B0, ref B7, 35);
                Mix(ref B2, ref B5, 56);
                Mix(ref B4, ref B3, 22);
                Mix(ref B0, ref B1, 46, K1, K2);
                Mix(ref B2, ref B3, 36, K3, K4);
                Mix(ref B4, ref B5, 19, K5, K6 + T1);
                Mix(ref B6, ref B7, 37, K7 + T2, K8 + 10);
                Mix(ref B2, ref B1, 33);
                Mix(ref B4, ref B7, 27);
                Mix(ref B6, ref B5, 14);
                Mix(ref B0, ref B3, 42);
                Mix(ref B4, ref B1, 17);
                Mix(ref B6, ref B3, 49);
                Mix(ref B0, ref B5, 36);
                Mix(ref B2, ref B7, 39);
                Mix(ref B6, ref B1, 44);
                Mix(ref B0, ref B7, 9);
                Mix(ref B2, ref B5, 54);
                Mix(ref B4, ref B3, 56);
                Mix(ref B0, ref B1, 39, K2, K3);
                Mix(ref B2, ref B3, 30, K4, K5);
                Mix(ref B4, ref B5, 34, K6, K7 + T2);
                Mix(ref B6, ref B7, 24, K8 + T0, K0 + 11);
                Mix(ref B2, ref B1, 13);
                Mix(ref B4, ref B7, 50);
                Mix(ref B6, ref B5, 10);
                Mix(ref B0, ref B3, 17);
                Mix(ref B4, ref B1, 25);
                Mix(ref B6, ref B3, 29);
                Mix(ref B0, ref B5, 39);
                Mix(ref B2, ref B7, 43);
                Mix(ref B6, ref B1, 8);
                Mix(ref B0, ref B7, 35);
                Mix(ref B2, ref B5, 56);
                Mix(ref B4, ref B3, 22);
                Mix(ref B0, ref B1, 46, K3, K4);
                Mix(ref B2, ref B3, 36, K5, K6);
                Mix(ref B4, ref B5, 19, K7, K8 + T0);
                Mix(ref B6, ref B7, 37, K0 + T1, K1 + 12);
                Mix(ref B2, ref B1, 33);
                Mix(ref B4, ref B7, 27);
                Mix(ref B6, ref B5, 14);
                Mix(ref B0, ref B3, 42);
                Mix(ref B4, ref B1, 17);
                Mix(ref B6, ref B3, 49);
                Mix(ref B0, ref B5, 36);
                Mix(ref B2, ref B7, 39);
                Mix(ref B6, ref B1, 44);
                Mix(ref B0, ref B7, 9);
                Mix(ref B2, ref B5, 54);
                Mix(ref B4, ref B3, 56);
                Mix(ref B0, ref B1, 39, K4, K5);
                Mix(ref B2, ref B3, 30, K6, K7);
                Mix(ref B4, ref B5, 34, K8, K0 + T1);
                Mix(ref B6, ref B7, 24, K1 + T2, K2 + 13);
                Mix(ref B2, ref B1, 13);
                Mix(ref B4, ref B7, 50);
                Mix(ref B6, ref B5, 10);
                Mix(ref B0, ref B3, 17);
                Mix(ref B4, ref B1, 25);
                Mix(ref B6, ref B3, 29);
                Mix(ref B0, ref B5, 39);
                Mix(ref B2, ref B7, 43);
                Mix(ref B6, ref B1, 8);
                Mix(ref B0, ref B7, 35);
                Mix(ref B2, ref B5, 56);
                Mix(ref B4, ref B3, 22);
                Mix(ref B0, ref B1, 46, K5, K6);
                Mix(ref B2, ref B3, 36, K7, K8);
                Mix(ref B4, ref B5, 19, K0, K1 + T2);
                Mix(ref B6, ref B7, 37, K2 + T0, K3 + 14);
                Mix(ref B2, ref B1, 33);
                Mix(ref B4, ref B7, 27);
                Mix(ref B6, ref B5, 14);
                Mix(ref B0, ref B3, 42);
                Mix(ref B4, ref B1, 17);
                Mix(ref B6, ref B3, 49);
                Mix(ref B0, ref B5, 36);
                Mix(ref B2, ref B7, 39);
                Mix(ref B6, ref B1, 44);
                Mix(ref B0, ref B7, 9);
                Mix(ref B2, ref B5, 54);
                Mix(ref B4, ref B3, 56);
                Mix(ref B0, ref B1, 39, K6, K7);
                Mix(ref B2, ref B3, 30, K8, K0);
                Mix(ref B4, ref B5, 34, K1, K2 + T0);
                Mix(ref B6, ref B7, 24, K3 + T1, K4 + 15);
                Mix(ref B2, ref B1, 13);
                Mix(ref B4, ref B7, 50);
                Mix(ref B6, ref B5, 10);
                Mix(ref B0, ref B3, 17);
                Mix(ref B4, ref B1, 25);
                Mix(ref B6, ref B3, 29);
                Mix(ref B0, ref B5, 39);
                Mix(ref B2, ref B7, 43);
                Mix(ref B6, ref B1, 8);
                Mix(ref B0, ref B7, 35);
                Mix(ref B2, ref B5, 56);
                Mix(ref B4, ref B3, 22);
                Mix(ref B0, ref B1, 46, K7, K8);
                Mix(ref B2, ref B3, 36, K0, K1);
                Mix(ref B4, ref B5, 19, K2, K3 + T1);
                Mix(ref B6, ref B7, 37, K4 + T2, K5 + 16);
                Mix(ref B2, ref B1, 33);
                Mix(ref B4, ref B7, 27);
                Mix(ref B6, ref B5, 14);
                Mix(ref B0, ref B3, 42);
                Mix(ref B4, ref B1, 17);
                Mix(ref B6, ref B3, 49);
                Mix(ref B0, ref B5, 36);
                Mix(ref B2, ref B7, 39);
                Mix(ref B6, ref B1, 44);
                Mix(ref B0, ref B7, 9);
                Mix(ref B2, ref B5, 54);
                Mix(ref B4, ref B3, 56);
                Mix(ref B0, ref B1, 39, K8, K0);
                Mix(ref B2, ref B3, 30, K1, K2);
                Mix(ref B4, ref B5, 34, K3, K4 + T2);
                Mix(ref B6, ref B7, 24, K5 + T0, K6 + 17);
                Mix(ref B2, ref B1, 13);
                Mix(ref B4, ref B7, 50);
                Mix(ref B6, ref B5, 10);
                Mix(ref B0, ref B3, 17);
                Mix(ref B4, ref B1, 25);
                Mix(ref B6, ref B3, 29);
                Mix(ref B0, ref B5, 39);
                Mix(ref B2, ref B7, 43);
                Mix(ref B6, ref B1, 8);
                Mix(ref B0, ref B7, 35);
                Mix(ref B2, ref B5, 56);
                Mix(ref B4, ref B3, 22);

                // final key schedule
                Output[0] = B0 + K0;
                Output[1] = B1 + K1;
                Output[2] = B2 + K2;
                Output[3] = B3 + K3;
                Output[4] = B4 + K4;
                Output[5] = B5 + K5 + T0;
                Output[6] = B6 + K6 + T1;
                Output[7] = B7 + K7 + 18;
            }
            #endregion

            #region Private Methods
            private static ulong RotateLeft64(ulong V, int B)
            {
                return (V << B) | (V >> (64 - B));
            }

            private static ulong RotateRight64(ulong V, int B)
            {
                return (V >> B) | (V << (64 - B));
            }

            private static void Mix(ref ulong A, ref ulong B, int R)
            {
                A += B;
                B = RotateLeft64(B, R) ^ A;
            }

            private static void Mix(ref ulong A, ref ulong B, int R, ulong K0, ulong K1)
            {
                B += K1;
                A += B + K0;
                B = RotateLeft64(B, R) ^ A;
            }

            private static void UnMix(ref ulong A, ref ulong B, int R)
            {
                B = RotateRight64(B ^ A, R);
                A -= B;
            }

            private static void UnMix(ref ulong A, ref ulong B, int R, ulong K0, ulong K1)
            {
                B = RotateRight64(B ^ A, R);
                A -= B + K0;
                B -= K1;
            }

            internal void SetTweak(ulong[] Tweak)
            {
                _expandedTweak[0] = Tweak[0];
                _expandedTweak[1] = Tweak[1];
                _expandedTweak[2] = Tweak[0] ^ Tweak[1];
            }

            internal void SetKey(ulong[] Key)
            {
                int i;
                ulong parity = KeyScheduleConst;

                for (i = 0; i < _expandedKey.Length - 1; i++)
                {
                    _expandedKey[i] = Key[i];
                    parity ^= Key[i];
                }

                _expandedKey[i] = parity;
            }
            #endregion
        }
        #endregion

        #region IDispose
        /// <summary>
        /// Dispose of this class
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool Disposing)
        {
            if (!m_isDisposed && Disposing)
            {
                try
                {
                    if (m_blockCipher != null)
                    {
                        m_blockCipher.Clear();
                        m_blockCipher = null;
                    }
                    if (m_cipherInput != null)
                    {
                        Array.Clear(m_cipherInput, 0, m_cipherInput.Length);
                        m_cipherInput = null;
                    }
                    if (m_digestState != null)
                    {
                        Array.Clear(m_digestState, 0, m_digestState.Length);
                        m_digestState = null;
                    }
                }
                finally
                {
                    m_isDisposed = true;
                }
            }
        }
        #endregion
    }
}
