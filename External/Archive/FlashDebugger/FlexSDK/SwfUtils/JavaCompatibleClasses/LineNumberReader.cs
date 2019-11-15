// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.IO;
using System.Text;

namespace JavaCompatibleClasses
{
    public class LineNumberReader : TextReader
    {
        private TextReader m_BaseReader;
        private int m_LineNumber;
        private const char LF = '\n';

        public LineNumberReader(TextReader reader)
        {
            m_BaseReader = reader;
            m_LineNumber = 0;
        }

        public int GetLineNumber()
        {
            return m_LineNumber;
        }

        public void SetLineNumber(int LineNumber)
        {
            m_LineNumber = LineNumber;
        }

        public override int Read()
        {
            int ch = m_BaseReader.Read();

            if (ch == LF)
            {
                m_LineNumber++;
            }

            return ch;
        }

        public override int Read(char[] buffer, int index, int count)
        {
            int countRead = m_BaseReader.Read(buffer, index, count);

            ScanBuffer(buffer, index, countRead);

            return countRead;
        }

        public override int ReadBlock(char[] buffer, int index, int count)
        {
            int countRead = m_BaseReader.ReadBlock(buffer, index, count);

            ScanBuffer(buffer, index, countRead);

            return countRead;
        }

        public override string ReadLine()
        {
            string line = m_BaseReader.ReadLine();

            if (line != null)
            {
                foreach (char ch in line)
                {
                    if (ch == LF)
                    {
                        m_LineNumber++;
                    }
                }
            }

            return line;
        }

        private void ScanBuffer(char[] buffer, int index, int count)
        {
            int indexEnd = index + count;

            for (int indexRead = index; indexRead < indexEnd; indexRead++)
            {
                if (buffer[indexRead] == LF)
                {
                    m_LineNumber++;
                }
            }
        }
    }
}
