using System.ComponentModel.DataAnnotations;

namespace BasicCrud.Enums;
public enum KeySignature
{
    [Display(Name = "C Major")]
    CMajor = 0,
    [Display(Name = "A Minor")]
    AMinor = 1,
    [Display(Name = "G Major")]
    GMajor = 2,
    [Display(Name = "E Minor")]
    EMinor = 3,
    [Display(Name = "D Major")]
    DMajor = 4,
    [Display(Name = "B Minor")]
    BMinor = 5,
    [Display(Name = "A Major")]
    AMajor = 6,
    [Display(Name = "F# Minor")]
    FSharpMinor = 7,
    [Display(Name = "E Major")]
    EMajor = 8,
    [Display(Name = "C# Minor")]
    CSharpMinor = 9,
    [Display(Name = "B Major")]
    BMajor = 10,
    [Display(Name = "G# Minor")]
    GSharpMinor = 11,
    [Display(Name = "F# Major")]
    FSharpMajor = 12,
    [Display(Name = "D# Minor")]
    DSharpMinor = 13,
    [Display(Name = "C# Major")]
    CSharpMajor = 14,
    [Display(Name = "A# Minor")]
    ASharpMinor = 15,
    [Display(Name = "F Major")]
    FMajor = 16,
    [Display(Name = "D Minor")]
    DMinor = 17,
    [Display(Name = "B♭ Major")]
    BFlatMajor = 18,
    [Display(Name = "G Minor")]
    GMinor = 19,
    [Display(Name = "E♭ Major")]
    EFlatMajor = 20,
    [Display(Name = "C Minor")]
    CMinor = 21,
    [Display(Name = "A♭ Major")]
    AFlatMajor = 22,
    [Display(Name = "F Minor")]
    FMinor = 23,
    [Display(Name = "D♭ Major")]
    DFlatMajor = 24,
    [Display(Name = "B♭ Minor")]
    BFlatMinor = 25,
    [Display(Name = "G♭ Major")]
    GFlatMajor = 26,
    [Display(Name = "E♭ Minor")]
    EFlatMinor = 27,
    [Display(Name = "A♭ Major")]
    AFlatMinor = 28
}