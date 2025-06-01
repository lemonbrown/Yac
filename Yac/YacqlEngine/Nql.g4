grammar Nql;

query       : playerQuery EOF ;

playerQuery : 'player:' NAME 'stats' whereClause? ;

whereClause : 'where' condition ( 'and' condition )* ;

condition   : NAME operator value ;

operator    : '=' | '>' | '<' | '>=' | '<=' ;

value       : NAME | NUMBER ;

NAME        : [a-zA-Z0-9_]+ ;
NUMBER      : [0-9]+ ;

WS          : [ \t\r\n]+ -> skip ;
