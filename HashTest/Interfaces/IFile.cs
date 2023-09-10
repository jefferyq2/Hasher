using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HasherTest.Interfaces
{
    public interface IFile
    {
        /// <summary>
        /// Name of the file.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// File size in bytes.
        /// </summary>
        long Size { get; }
        /// <summary>
        /// Location of the file.
        /// </summary>
        string Path { get; }
        /// <summary>
        /// Creation date of the file.
        /// </summary>
        DateTime CreatedOn { get; }

    }
}
