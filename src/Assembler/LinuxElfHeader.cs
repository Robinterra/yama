namespace Yama.Assembler
{

    public class LinuxElfHeader
    {

        #region get/set

        public byte[] EI_NIDENT = new byte[]
        {
            0x7f,//EI_MAG0
            0x45,//EI_MAG1
            0x4C,//EI_MAG2
            0x46,//EI_MAG3
            (byte)EI_Class.ELFCLASSNONE,
            (byte)EI_DATA.ELFDATA2LSB,
            1,//EI_VERSION
            (byte)EI_OSABI.ELFOSABI_NONE,
            0,//EI_ABIVERSION
            0,//EI_PAD
            0,
            0,
            0,
            0,
            0,
            0
        };

        public E_Type Type //Half
        {
            get;
            set;
        }

        public E_Machine Machine //Half
        {
            get;
            set;
        }

        public List<ElfProgramHeader> ProgramHeaders
        {
            get;
        }

        public List<ElfSectionHeader> SectionHeaders
        {
            get;
        }

        public uint Size
        {
            get
            {
                return (uint)this.EI_NIDENT.Length + 0x24 + (uint)this.ProgramHeaders.Count * ElfProgramHeader.Size + 0xc;
            }
        }

        #endregion get/set

        #region ctor

        public LinuxElfHeader(EI_Class eiClass, E_Machine eMachine)
        {
            this.EI_NIDENT[4] = (byte)eiClass;
            this.ProgramHeaders = new List<ElfProgramHeader>();
            this.SectionHeaders = new List<ElfSectionHeader>();
            this.Type = E_Type.ET_EXEC;
            this.Machine = eMachine;
        }

        #endregion ctor

        #region methods

        public uint StreamData(Stream stream, uint startAdress, uint programSize)
        {
            uint size = this.Size;

            stream.Write(this.EI_NIDENT);

            Span<byte> span = stackalloc byte[0x24];
            BitConverter.TryWriteBytes(span.Slice(0x00), (ushort)this.Type);
            BitConverter.TryWriteBytes(span.Slice(0x02), (ushort)this.Machine);
            BitConverter.TryWriteBytes(span.Slice(0x04), (uint)1);//e_version
            BitConverter.TryWriteBytes(span.Slice(0x08), (uint)size | startAdress);//e_entry
            BitConverter.TryWriteBytes(span.Slice(0x0c), (uint)this.EI_NIDENT.Length + 0x24);//e_phoff
            BitConverter.TryWriteBytes(span.Slice(0x10), (uint)this.SectionHeaders.Count);//e_shoff, bei falschen größen hier, kann das programm nicht mit gängigen reverse tools gelesen werden
            BitConverter.TryWriteBytes(span.Slice(0x14), (uint)E_Flags.EF_MIR_NICHT_BEKANNT | (uint)E_Flags.EF_SPARC_SUN_US1);
            BitConverter.TryWriteBytes(span.Slice(0x18), (ushort)(this.EI_NIDENT.Length + 0x24));//e_ehsize
            BitConverter.TryWriteBytes(span.Slice(0x1a), (ushort)ElfProgramHeader.Size);//e_phentsize
            BitConverter.TryWriteBytes(span.Slice(0x1c), (ushort)this.ProgramHeaders.Count);//e_phnum
            BitConverter.TryWriteBytes(span.Slice(0x1e), (ushort)ElfSectionHeader.Size);//e_shentsize
            BitConverter.TryWriteBytes(span.Slice(0x20), (ushort)this.SectionHeaders.Count);//e_shnum
            BitConverter.TryWriteBytes(span.Slice(0x22), (ushort)0x03);//e_shstrndx

            stream.Write(span);

            //phent Program Header Table
            foreach (ElfProgramHeader ph in this.ProgramHeaders)
            {
                ph.StreamData(stream);
            }

            stream.Write(stackalloc byte[0xc]);

            return startAdress;
        }

        #endregion methods

        public enum E_Flags
        {
            EF_SPARCV9_TSO,
            EF_SPARCV9_PSO,
            EF_SPARCV9_RMO,
            EF_SPARCV9_MM,
            EF_SPARC_EXT_MASK = 0xffff00,
            EF_SPARC_SUN_US1 = 0x000200,
            EF_SPARC_HAL_R1 = 0x000400,
            EF_SPARC_SUN_US3 = 0x000800,
            EF_MIR_NICHT_BEKANNT = 0x5000000

        }

        public enum EI_OSABI
        {
            ELFOSABI_NONE,
            ELFOSABI_HPUX,
            ELFOSABI_NETBSD,
            ELFOSABI_LINUX,
            ELFOSABI_SOLARIS,
            ELFOSABI_AIX,
            ELFOSABI_IRIX,
            ELFOSABI_FREEBSD,
            ELFOSABI_TRU64,
            ELFOSABI_MODESTO,
            ELFOSABI_OPENBSD,
            ELFOSABI_OPENVMS,
            ELFOSABI_NSK
        }

        public enum EI_DATA
        {
            ELFDATANONE,
            ELFDATA2LSB,
            ELFDATA2MSB
        }

        public enum EI_Class
        {
            ELFCLASSNONE,
            ELFCLASS32,
            ELFCLASS64

        }

        public enum E_Type
        {
            ET_NONE,
            ET_REL,
            ET_EXEC,
        }

        public enum E_Machine
        {
            EM_NONE,
            EM_M32,
            EM_SPARC,
            EM_386,
            EM_68K,
            EM_88K,
            FUTURE_0,
            EM_860,
            EM_ARM = 40,//32Bit
            EM_X86_64 = 62,//64Bit AMD
            EM_AARCH64 = 183,//64Bit
            EM_RISCV = 243,
        }

    }

    public class ElfSectionHeader
    {


        public const ushort Size = 0x28;

        #region get/set

        #endregion get/set

        #region ctor

        public ElfSectionHeader()
        {

        }

        #endregion ctor

    }

    public class ElfProgramHeader
    {

        public const ushort Size = 0x20;

        #region get/set

        public P_Types Type
        {
            get;
            set;
        }

        public uint VAddresse
        {
            get;
            set;
        }

        public uint PAddresse
        {
            get;
            set;
        }

        public uint FileSize
        {
            get;
            set;
        }

        public uint MemorySize
        {
            get;
            set;
        }

        public uint PAlign
        {
            get;
            set;
        }

        #endregion get/set

        #region ctor

        public ElfProgramHeader()
        {
            this.Type = P_Types.PT_LOAD;
        }

        #endregion ctor

        #region methods

        public uint StreamData(Stream stream)
        {
            Span<byte> span = stackalloc byte[Size];
            BitConverter.TryWriteBytes(span.Slice(0x00), (uint)this.Type);//p_type
            BitConverter.TryWriteBytes(span.Slice(0x04), (uint)0x0);//p_offset
            BitConverter.TryWriteBytes(span.Slice(0x08), (uint)this.VAddresse);//p_vaddr
            BitConverter.TryWriteBytes(span.Slice(0x0c), (uint)this.PAddresse);//p_paddr
            BitConverter.TryWriteBytes(span.Slice(0x10), (uint)this.FileSize);//p_filesz (uint)programSize - startAdress
            BitConverter.TryWriteBytes(span.Slice(0x14), (uint)this.MemorySize);//p_memsz
            BitConverter.TryWriteBytes(span.Slice(0x18), (uint)P_Flags.PF_R | (uint)P_Flags.PF_X | (uint)P_Flags.PF_W);//p_flags
            BitConverter.TryWriteBytes(span.Slice(0x1c), (uint)this.PAlign);//p_align

            stream.Write(span);

            return Size;
        }

        #endregion methods

        public enum P_Types
        {
            PT_NULL = 0,
            PT_LOAD = 1,
            PT_DYNAMIC = 2,
        }

        public enum P_Flags
        {
            None,
            PF_X = 1,
            PF_W = 2,
            PF_R = 4,

        }

    }

}