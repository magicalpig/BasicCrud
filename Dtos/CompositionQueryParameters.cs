using BasicCrud.Enums;

namespace BasicCrud.Dtos;

    public class CompositionQueryParameters()
    {
        public string? Name { get; set; }
        public KeySignature? KeySignature { get; set; }
        public string? KeySignatureDisplayName { get; set;}
        public Format? Format { get; set; }
        public string? ComposerName { get; set; }
    }