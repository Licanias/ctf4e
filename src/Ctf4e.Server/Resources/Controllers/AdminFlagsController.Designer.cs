﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ctf4e.Server.Resources.Controllers {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class AdminFlagsController {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AdminFlagsController() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Ctf4e.Server.Resources.Controllers.AdminFlagsController", typeof(AdminFlagsController).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid input..
        /// </summary>
        internal static string CreateFlagAsync_InvalidInput {
            get {
                return ResourceManager.GetString("CreateFlagAsync:InvalidInput", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The flag was created successfully..
        /// </summary>
        internal static string CreateFlagAsync_Success {
            get {
                return ResourceManager.GetString("CreateFlagAsync:Success", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occured when creating the flag. Check the log for more details..
        /// </summary>
        internal static string CreateFlagAsync_UnknownError {
            get {
                return ResourceManager.GetString("CreateFlagAsync:UnknownError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The flag was deleted successfully..
        /// </summary>
        internal static string DeleteFlagAsync_Success {
            get {
                return ResourceManager.GetString("DeleteFlagAsync:Success", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occured when deleting the flag. Check the log for more details..
        /// </summary>
        internal static string DeleteFlagAsync_UnknownError {
            get {
                return ResourceManager.GetString("DeleteFlagAsync:UnknownError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid input..
        /// </summary>
        internal static string EditFlagAsync_InvalidInput {
            get {
                return ResourceManager.GetString("EditFlagAsync:InvalidInput", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The flag was updated successfully..
        /// </summary>
        internal static string EditFlagAsync_Success {
            get {
                return ResourceManager.GetString("EditFlagAsync:Success", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occured when updating the flag. Check the log for more details..
        /// </summary>
        internal static string EditFlagAsync_UnknownError {
            get {
                return ResourceManager.GetString("EditFlagAsync:UnknownError", resourceCulture);
            }
        }
    }
}
