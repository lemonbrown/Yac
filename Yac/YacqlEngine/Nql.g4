grammar Nql;

query        : playerQuery
             | teamQuery
             | gameQuery
             ;

playerQuery  : 'player:' NAME 'stats' whereClause? ;
teamQuery    : 'team:' NAME ( 'schedule' | 'record' ) whereClause? ;
gameQuery    : 'games' whereClause ;

whereClause  : 'where' condition ( 'and' condition )* ;
condition    : NAME operator value ;
operator     : '=' | '>' | '<' | '>=' | '<=' ;
value        : NAME | NUMBER ;

NAME         : [a-zA-Z0-9_]+ ;
NUMBER       : [0-9]+ ;
WS           : [ \t\r\n]+ -> skip ;
