using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Yama.Compiler.Definition
{
    public class DefinitionManager
    {

        // -----------------------------------------------

        #region get/set

        // -----------------------------------------------

        public List<IProcessorDefinition> AviableDefinitions
        {
            get;
            set;
        }

        // -----------------------------------------------

        #endregion get/set

        // -----------------------------------------------

        #region ctor

        // -----------------------------------------------

        public DefinitionManager (  )
        {
            this.Init (  );
        }

        // -----------------------------------------------

        #endregion ctor

        // -----------------------------------------------

        #region methods

        // -----------------------------------------------

        private bool Init (  )
        {
            this.AviableDefinitions = new List<IProcessorDefinition>();

            this.GetAllDefinitions (  );

            return true;
        }

        // -----------------------------------------------

        public bool PrintAllDefinitions (  )
        {
            Console.WriteLine ( "Folgende definitionen sind verfügbar:" );

            foreach ( IProcessorDefinition def in this.AviableDefinitions )
            {
                Console.WriteLine ( def.Name );
            }

            return true;
        }

        // -----------------------------------------------

        public bool PrintSimpleError(string text)
        {
            ConsoleColor colr = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.Error.WriteLine ( text );

            Console.ForegroundColor = colr;

            return false;
        }

        // -----------------------------------------------

        private bool GetAllDefinitions()
        {
            DirectoryInfo defsFolder = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "defs"));

            if (!defsFolder.Exists) return this.PrintSimpleError("Es konnten keine Definitionen gefunden werden");

            foreach ( FileInfo file in defsFolder.GetFiles (  ) )
            {
                IProcessorDefinition def = this.DeserializesDef ( file );

                if (def == null) continue;

                this.AviableDefinitions.Add ( def );
            }

            return true;
        }

        // -----------------------------------------------

        public IProcessorDefinition GetDefinition ( string name )
        {
            IProcessorDefinition def = this.AviableDefinitions.FirstOrDefault ( t => t.Name == name );

            if (def == null) this.PrintSimpleError(string.Format("The definition {0} can not be found!", name));

            return def;
        }

        // -----------------------------------------------

        private IProcessorDefinition DeserializesDef ( FileInfo file )
        {
            if ( !file.Exists ) return null;
            if ( file.Extension != ".json" ) return null;

            using ( FileStream stream = file.OpenRead (  ) )

            return JsonSerializer.DeserializeAsync<GenericDefinition> ( stream ).Result;
        }

        // -----------------------------------------------

        #endregion methods

        // -----------------------------------------------

    }
}