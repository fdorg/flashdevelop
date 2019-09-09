/*
 * Required Interface between ASClassParser 
 * and the application using the parser
 */

using System;
using System.Collections;

namespace ASCompletion
{
    public interface IASContext
    {
        #region methods
        
        /// <summary>
        /// Add the current class' base path to the classpath
        /// </summary>
        /// <param name="fileName">Relative to this file</param>
        /// <param name="basePath">Resolved this base path</param>
        void SetTemporaryBasePath(string fileName, string basePath);
        
        /// <summary>
        /// Retrieves a parsed class from its name
        /// </summary>
        /// <param name="cname">Class (short or full) name</param>
        /// <param name="inClass">Current class</param>
        /// <returns>A parsed class or an empty ASClass if the class is not found</returns>
        ASClass GetClassByName(string cname, ASClass inClass);
        
        /// <summary>
        /// Retrieves a parsed class from its filename
        /// </summary>
        /// <param name="fileName">Class' file name</param>
        /// <returns>A parsed class or an empty ASClass if the class is not found or invalid</returns>
        ASClass GetClassByFile(string fileName);
        
        /// <summary>
        /// Retrieves the current active class
        /// </summary>
        /// <returns>ASClass objet</returns>
        ASClass GetCurrentClass();
                
        /// <summary>
        /// Find folder and classes in classpath
        /// </summary>
        /// <param name="folder">Path to eval</param>
        /// <returns>Package folders and classes</returns>
        ASMemberList GetSubClasses(string folder);
        
        /// <summary>
        /// (Re)Parse (if necessary) and cache a class file
        /// </summary>
        /// <param name="aClass">Class object</param>
        /// <returns>The class object</returns>
        ASClass GetCachedClass(ASClass aClass);
        
        /// <summary>
        /// Resolve wildcards in imports
        /// </summary>
        /// <param name="package">Package to explore</param>
        /// <param name="inClass">Current class</param>
        /// <param name="known">Packages already added</param>
        void ResolveWildcards(string package, ASClass inClass, ArrayList known);
        
        /// <summary>
        /// Depending on the context UI, display some message
        /// </summary>
        void DisplayError(string message);
        #endregion
    }
}
