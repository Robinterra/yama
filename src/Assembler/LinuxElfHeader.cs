namespace Yama.Assembler
{

    public class LinuxElfHeader
    {

        #region get/set

        byte[] EI_NIDENT = new byte[]
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

        E_Type Type //Half
        {
            get;
            set;
        }

        E_Machine Machine //Half
        {
            get;
            set;
        }

        public uint Size;

        #endregion get/set

        #region ctor

        public LinuxElfHeader(EI_Class eiClass, E_Machine eMachine)
        {
            this.EI_NIDENT[4] = (byte)eiClass;
            this.Type = E_Type.ET_EXEC;
            this.Machine = eMachine;
            this.Size = 0x60;
        }

        #endregion ctor

        #region methods

        public uint StreamData(Stream stream, uint startAdress, uint programSize)
        {
            uint size = this.Size;
            startAdress -= size;

            stream.Write(this.EI_NIDENT);
            stream.Write(BitConverter.GetBytes((ushort)this.Type));
            stream.Write(BitConverter.GetBytes((ushort)this.Machine));
            stream.Write(BitConverter.GetBytes((uint)1));//e_version
            stream.Write(BitConverter.GetBytes((uint)size | startAdress));//e_entry
            stream.Write(BitConverter.GetBytes((uint)0x34));//e_phoff
            stream.Write(BitConverter.GetBytes((uint)size | startAdress | 0xc));//e_shoff
            stream.Write(BitConverter.GetBytes((uint)E_Flags.EF_MIR_NICHT_BEKANNT | (uint)E_Flags.EF_SPARC_SUN_US1));
            stream.Write(BitConverter.GetBytes((ushort)0x34));//e_ehsize
            stream.Write(BitConverter.GetBytes((ushort)0x20));//e_phentsize
            stream.Write(BitConverter.GetBytes((ushort)0x01));//e_phnum
            stream.Write(BitConverter.GetBytes((ushort)0x28));//e_shentsize
            stream.Write(BitConverter.GetBytes((ushort)0x04));//e_shnum
            stream.Write(BitConverter.GetBytes((ushort)0x03));//e_shstrndx

            //phent Program Header Table
            stream.Write(BitConverter.GetBytes((uint)0x1));//p_type
            stream.Write(BitConverter.GetBytes((uint)0x0));//p_offset
            stream.Write(BitConverter.GetBytes((uint)startAdress));//p_vaddr
            stream.Write(BitConverter.GetBytes((uint)startAdress));//p_paddr
            stream.Write(BitConverter.GetBytes((uint)programSize - startAdress));//p_filesz (uint)programSize - startAdress
            stream.Write(BitConverter.GetBytes((uint)programSize - startAdress));//p_memsz
            stream.Write(BitConverter.GetBytes((uint)P_Flags.PF_R | (uint)P_Flags.PF_X | (uint) P_Flags.PF_W));//p_flags
            stream.Write(BitConverter.GetBytes((uint)startAdress));//p_align

            stream.Write(new byte[0xc]);

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
            EM_ARM = 40
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