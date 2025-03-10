namespace FriedMacroCode;

public enum Token
{
    Bang,                       //  !
    ApeTail,                    //  @
    lPar,                       //  (
    rPar,                       //  )
    lBrace,                     //  {
    rBrace,                     //  }


    String,                     // "anything here"
    Embed,                      // #anything here

    Comment,                    // //comment or /* comment */
    Keyword,                    // var if else ( as long as its in list)
    Identifier,                 // myVarible (not in list)

    BadToken,                   //  Token not found
    EOF,                        //  End of the file
}
