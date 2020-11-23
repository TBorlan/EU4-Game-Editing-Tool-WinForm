using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using EU4GET_WF.SerDes.FileParsing.Internal;

namespace EU4GET_WF.SerDes.FileParsing
{
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

        public static TextNode ReadFile(string filePath)
        {
            InitialiseReader();
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
