﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ctf4e.LabServer.Resources.Controllers {
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
    internal class AdminConfigurationController_de {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AdminConfigurationController_de() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Ctf4e.LabServer.Resources.Controllers.AdminConfigurationController.de", typeof(AdminConfigurationController_de).Assembly);
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
        ///   Looks up a localized string similar to Für die Konfigurationsdatei sind keine Schreibrechte gesetzt. Diese kann daher nur gelesen, aber nicht verändert werden..
        /// </summary>
        internal static string RenderAsync_ConfigNonWritable {
            get {
                return ResourceManager.GetString("RenderAsync:ConfigNonWritable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Beim Lesen der Konfigurationsdatei ist ein Fehler aufgetreten. Weitere Details finden sich im Log..
        /// </summary>
        internal static string RenderAsync_UnknownError {
            get {
                return ResourceManager.GetString("RenderAsync:UnknownError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Doppelte Aufgabe-ID: {0}.
        /// </summary>
        internal static string UpdateConfigurationAsync_DuplicateExerciseId {
            get {
                return ResourceManager.GetString("UpdateConfigurationAsync:DuplicateExerciseId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bei der Validierung sind Fehler aufgetreten. Die neue Konfiguration wurde nicht gespeichert..
        /// </summary>
        internal static string UpdateConfigurationAsync_Error {
            get {
                return ResourceManager.GetString("UpdateConfigurationAsync:Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Konnte neue Konfiguration nicht parsen. Ist das JSON gültig? Weitere Details finden sich im Log..
        /// </summary>
        internal static string UpdateConfigurationAsync_ErrorParsingNewConfig {
            get {
                return ResourceManager.GetString("UpdateConfigurationAsync:ErrorParsingNewConfig", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Die Konfiguration wurde erfolgreich aktualisiert..
        /// </summary>
        internal static string UpdateConfigurationAsync_Success {
            get {
                return ResourceManager.GetString("UpdateConfigurationAsync:Success", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Beim Aktualisieren der Konfiguration ist ein Fehler aufgetreten. Möglicherweise ist das System jetzt in einem inkonsistenten Zustand. Es wird empfohlen, die aktuelle Konfiguration zu prüfen und einen Neustart durchzuführen. Weitere Details finden sich im Log..
        /// </summary>
        internal static string UpdateConfigurationAsync_UnknownError {
            get {
                return ResourceManager.GetString("UpdateConfigurationAsync:UnknownError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bei der Validierung wurde ein Fehler erkannt: {0}.
        /// </summary>
        internal static string UpdateConfigurationAsync_ValidationError {
            get {
                return ResourceManager.GetString("UpdateConfigurationAsync:ValidationError", resourceCulture);
            }
        }
    }
}
