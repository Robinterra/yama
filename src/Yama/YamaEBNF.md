letter = §"a-zA-Z"
digit = §"0-9"
hexLetter = §"a-f"
operator = §"+-*/<>~%&|!" | "==" | ">=" | "<="

identiefier = letter{letter | digit | _}
number = digit {digit}
hexnumber = "0x" (digit | hexletter) {digit | hexletter}
pointIdentifion = identiefier {.identiefier}

asExpression = "as" identiefier
isExpression = "is" (null | identiefier identiefier)

ifExpression = [ operator ] pointIdentifion [ asExpression | isExpression | methodeCall ] { operationExpression }

operationExpression = operator expression
expression = [ operator ] pointIdentifion [ asExpression | methodeCall ] { operationExpression }

genericCall = "<" identiefier {, identiefier} ">"
methodeCall = [ genericCall ] "(" expression {, expression} ")" [ asExpression ]

newStatement = "new" identiefier "(" expression {, expression} ")" [ asExpression ]
assigment = "=" (expression | newStatement)

normalStatement = pointIdentifion ( methodeCall | assigment )
returnStatement = expression ";"

ifStatement = "if" "(" ifConditionExpression ")" statementInMethode


statementInMehtode = normalStatement | ifStatement | whileStatement | forStatement | returnStatement