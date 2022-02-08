using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EU4GET_WF.SerDes.FileParsing.Internal;

namespace EU4GET_WF.SerDes.FileParsing
{
    /// <summary>
    /// Contains static methods to parse a given file entry.
    /// </summary>
    /// <remarks>
    /// The class contains two methods for parsing:
    /// <see cref="ReadFile"/> is the synchronous method.
    /// <see cref="ReadFileAsync"/> is the asynchronous method.
    /// Both functions return a <see cref="TextNode"/> as the root of the parsed data tree, and the <see cref="TextNode._mValue"/>
    /// will be the name of the file parsed.
    /// </remarks>
    static class ReaderFactory
    {
        private static FileReader _mFileReader;
        private static readonly List<String> _mFilePaths = new List<string>(3) { null, null, null };
        private static readonly SemaphoreSlim _mSemaphore = new SemaphoreSlim(3);
        private static SpinLock _mLockObject = new SpinLock();
        private static bool _mLockTaken = false;
        private static void InitialiseReader()
        {
            if (_mFileReader == null)
            {
                _mFileReader = new FileReader();

            }
        }

        /// <summary>
        /// Parses a given file in a synchronous way and returns a hierarchical tree containing data.
        /// </summary>
        /// <param name="filePath">Absolute path to the file.</param>
        /// <returns><see cref="TextNode"/> as the root container of the data tree</returns>
        /// <exception cref="Exception">If <param name="filePath"/> is not a file type supported, then an exception will be raised.</exception>
        public static TextNode ReadFile(string filePath)
        {
            InitialiseReader();
            //TODO: Check for exception
            string extension = Path.GetExtension(filePath);
            if (extension.Equals(".txt") || extension.Equals(".csv"))
            {
                return _mFileReader.ReadFile(filePath);
            }
            else
            {
                string message = extension + " file extension not supported";
                throw new Exception(message);
            }
        }

        /// <summary>
        /// Parses a given file in a asynchronous way and returns a hierarchical tree containing data.
        /// </summary>
        /// <remarks> Task should be called outside of the main thread</remarks>
        /// <param name="filePath">Absolute path to the file.</param>
        /// <returns><see cref="TextNode"/> as the root container of the data tree</returns>
        /// <exception cref="Exception">If <param name="filePath"/> is not a file type supported, then an exception will be raised.</exception>
        public static async Task<TextNode> ReadFileAsync(string filePath)
        {
            _mSemaphore.Wait();
            int i;
            try
            {
                _mLockObject.Enter(ref _mLockTaken);
                for (i = 0; i < 3; i++)
                {
                    if (_mFilePaths[i] == null)
                    {
                        _mFilePaths[i] = String.Copy(filePath);
                        break;
                    }
                }
            }
            finally
            {
                if (_mLockTaken)
                {
                    _mLockObject.Exit();
                    _mLockTaken = false;
                }
            }
            TextNode node = null;
            try
            {
                node = await Task<TextNode>.Run(() =>
                {
                    string extension = Path.GetExtension(_mFilePaths[i]);
                    if (extension.Equals(".txt") || extension.Equals(".csv"))
                    {
                        FileReader reader = new FileReader();
                        return reader.ReadFile(_mFilePaths[i]);
                    }
                    else
                    {
                        string message = extension + " file extension not supported";
                        throw new Exception(message);
                    }
                });
            }
            catch(AggregateException ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            finally
            {
                _mFilePaths[i] = null;
                _mSemaphore.Release();
            }
            return node;
        }
    }
}
