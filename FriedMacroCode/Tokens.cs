namespace FriedMacroCode;

public enum Token
{
    Bang,                       //  !
    ApeTail,                    //  @
    lPar,                       //  (
    rPar,                       //  )
    Percentage,                 //  %
    Comma,                      //  ,


    String,                     // "anything here"
    Embed,                      // #anything here

    XMLMacro,                   // <macro>any macro text here</macro>
    XMLCodeLua,                 // <lua>any lua code here</lua>
    XMLRaw,                     // <raw>anything here</raw>

    Comment,                    // //comment or /* comment */
    Keyword,                    // var if else ( as long as its in list)
    Identifier,                 // myVarible (not in list)

    BadToken,                   //  Token not found
    EOF,                        //  End of the file
}
