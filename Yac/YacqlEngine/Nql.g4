grammar Nql;

query        : playerQuery
             | teamQuery
             | gameQuery
             | compareQuery
             | seasonQuery
             ;

playerQuery  : 'player:' NAME 'stats' whereClause? ;
teamQuery    : 'team:' NAME ( 'schedule' | 'record' ) whereClause? ;
gameQuery    : 'games' whereClause ;
compareQuery
  : 'compare' compareTarget fieldSelection? whereClause?
  ;
seasonQuery  : 'seasons' fieldSelection? whereClause? ;

compareTarget
    : 'players' ':' playerList
    | 'teams' ':' teamList
    | 'games' ':' gameList
    | teamList // optionally omit prefix for now
    ;

playerList  : NAME (',' NAME)* ;
teamList    : NAME (',' NAME)* ;
gameList    : NAME (',' NAME)* ;

fieldSelection : field ((',' | WS) field)* ;
field
    : AGGREGATE NAME       # totalField
    | NAME                 # nameField
    ;


whereClause  : 'where' condition ( 'and' condition )* ;
condition    : NAME operator value ;
operator     : '=' | '>' | '<' | '>=' | '<=' ;
value        : NAME | NUMBER | STRING ;

AGGREGATE    : 'total' ; // Add more later like 'avg', 'sum', etc.
NAME         : [a-zA-Z0-9_]+ ;
NUMBER       : [0-9]+ ;
WS           : [ \t\r\n]+ -> skip ;
STRING       : '\'' (~['\r\n])* '\'' ; 



