grammar Nql;

query        : playerQuery
             | teamQuery
             | gameQuery             
             | seasonQuery
             ;

playerQuery  : 'player:' NAME 'stats' whereClause? ;
teamQuery    : ('team:' NAME | 'teams') fieldSelection? whereClause? ;
gameQuery    : 'games' whereClause ;
seasonQuery  : 'seasons' fieldSelection? whereClause? ;

playerList  : NAME (',' NAME)* ;
teamList    : NAME (',' NAME)* ;
gameList    : NAME (',' NAME)* ;

fieldSelection : field ((',' | WS) field)* ;
field
    : ('total' | 'avg' | 'most' | 'least') NAME  # aggregateField
    | NAME                  # nameField
    ;


whereClause  : 'where' condition ( 'and' condition )* ;
condition    : NAME operator value ;
operator     : '=' | '>' | '<' | '>=' | '<=' ;
value        : NAME | NUMBER | STRING ;

NAME         : [a-zA-Z0-9_]+ ;
NUMBER       : [0-9]+ ;
STRING       : '\'' (~['\r\n])* '\'' ;
WS           : [ \t\r\n]+ -> skip ;


