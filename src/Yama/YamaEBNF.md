letter = §"a-zA-Z"
digit = §"0-9"
hexLetter = §"a-f"
operator = §"+-*/<>~%&|!" | "==" | ">=" | "<=" | "&&" | "||"

identiefier = letter{letter | digit | _}
number = digit {digit}
hexnumber = "0x" (digit | hexletter) {digit | hexletter}
pointIdentifion = identiefier {.identiefier}

asExpression = "as" identiefier
isExpression = "is" (null | identiefier identiefier)



operationExpression = operator expression
expression = [ operator ] pointIdentifion [ asExpression | methodeCall ] { operationExpression }

genericCall = "<" identiefier {, identiefier} ">"
methodeCall = [ genericCall ] "(" expression {, expression} ")" [ asExpression ]

newStatement = "new" identiefier "(" expression {, expression} ")" [ asExpression ]
assigment = "=" (expression | newStatement)

normalStatement = pointIdentifion ( methodeCall | assigment ) | identiefier identiefier [ assigment ]
returnStatement = expression ";"

statementInLoop = statementInMehtode | continue | break
containerLoopStatement = "{" { statementInLoop [ ; ] } "}"
inLoopStatement = containerLoopStatement | ifStatement | whileStatement | forStatement | returnStatement | continue | break

conditionExpression = [ operator ] pointIdentifion [ asExpression | isExpression | methodeCall ] { operationExpression }

inIfStatement = containerStatement | ifStatement | whileStatement | forStatement | returnStatement
ifStatement = "if" "(" conditionExpression ")" inIfStatement [ else inIfStatement ]

whileStatement = "while" "(" conditionExpression ")" inLoopStatement
forStatement = "for" "(" normalStatement ";" conditionExpression ";" normalStatement ")" inLoopStatement
loopStatement = "loop" inLoopStatement

statementInMehtode = normalStatement | ifStatement | whileStatement | forStatement | returnStatement
containerStatement = "{" { statementInMehtode [ ; ] } "}"
getsetContain = "{" [ get containerStatement ] [ set containerStatement ] "}"

accessdefinition = "public" | "private"
methodeZusatz = "static" | "copy"
methodeDeklaration = [ accessdefinition ] [ methodeZusatz ] identiefier "(" { identiefier identieefier [ "," ] } ")" containerStatement
gobalvariableDeklaration = [ accessdefinition ] [ methodeZusatz ] identiefier ";"
propertiesDeklaration = [ accessdefinition ] [ methodeZusatz ] identiefier getsetContain
vektorDeklaration = [ accessdefinition ] [ methodeZusatz ] identiefier "[" identiefier identieefier [ "," ] { identiefier identieefier [ "," ] } "]" getsetContain

