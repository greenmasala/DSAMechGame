public enum SoundType
{
    None,
    Artillery_Shot,
    // [Ben] Filly, you forgot to change this one in Test.cs, it was crashing the main scene so I fixed it roughly
    // delete this + Test.cs when you don't need them anymore
    TestSound,
    // [Ben] My Jeep sounds, they're silent until you put clips in the SoundDatabase
    Jeep_Beep,
    Jeep_Explode,
}