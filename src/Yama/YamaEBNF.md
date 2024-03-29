letter = §"a-zA-Z"
digit = §"0-9"
hexLetter = §"a-f"
operator = §"+-*/<>~%&|!" | "==" | ">=" | "<=" | "&&" | "||"
string = '"' {allChars} '"'

identiefier = letter{letter | digit | _}
number = digit {digit}
hexnumber = "0x" (digit | hexletter) {digit | hexletter}
pointIdentifion = identiefier.identiefier
expressionIden = pointIdentifion | number | hexnumber | "true" | "false" | "null"

asExpression = "as" identiefier
isExpression = ("is"|"isnot") (null | [&]identiefier identiefier)

operationExpression = operator expression
expression = [ "(" ] [ operator ] expressionIden [ asExpression | methodeCall | vektorCall ] [ operationExpression ] [ ")" ] [ asExpression ] [ operationExpression ]

genericCall = "<" identiefier {, identiefier} ">"
methodeCall = [ genericCall ] "(" expression {, expression} ")" [ asExpression ]
vektorCall = "[" expression {, expression} "]" [ asExpression ]

newStatement = "new" identiefier "(" expression {, expression} ")" [ asExpression ]
typeofStatement = "typeof" "<" identiefier ">"
assigment = "=" (expression | newStatement | typeofStatement)

variableDeklaration = [&]identiefier identiefier

normalStatement = (pointIdentifion ( methodeCall | assigment | vektorCall ) | variableDeklaration [ assigment ] | identiefier assigment) ";"
returnStatement = "return" expression ";"

statementInLoop = statementInMehtode | continue | break
containerLoopStatement = "{" { statementInLoop [ ; ] } "}"
inLoopStatement = containerLoopStatement | ifStatement | whileStatement | forStatement | returnStatement | continue | break

conditionExpression = [ operator ] expressionIden [ asExpression | isExpression | methodeCall | vektorCall ] { operationExpression }

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

methodeDeklaration = [ accessdefinition ] [ methodeZusatz ] [&]identiefier identiefier "(" { variableDeklaration [ "," ] } ")" containerStatement
gobalvariableDeklaration = [ accessdefinition ] [ methodeZusatz ] [&]identiefier identiefier ";"
propertiesDeklaration = [ accessdefinition ] [ methodeZusatz ] [&]identiefier identiefier getsetContain
vektorDeklaration = [ accessdefinition ] [ methodeZusatz ] [&]identiefier identiefier "[" { variableDeklaration [ "," ] } "]" getsetContain

inclass = { methodeDeklaration | gobalvariableDeklaration | propertiesDeklaration | vektorDeklaration }
inenum = { identiefier "=" number | hexnumber [,] }

classDeklaration = accessdefinition "class" identiefier "{" inclass "}"
structDeklaration = accessdefinition "struct" identiefier "{" inclass "}"
EnumDeklaration = accessdefinition "enum" identiefier "{" inenum "}"
usingUse = "using" string ";"

innamespace = { classDeklaration | enumDeklaration | usingUse | structDeklaration }

namespaceDeklaration = "namespace" string "{" innamespace "}"