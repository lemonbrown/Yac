grammar Nql;

query        : playerQuery
             | teamQuery
             | gameQuery             
             | seasonQuery
             ;

playerQuery  : 'player:' NAME 'stats' whereClause? ;
teamQuery    : ('team:' NAME | 'teams') fieldSelection? groupingClause? whereClause? ;
gameQuery    : 'games' fieldSelection? groupingClause? whereClause? ;
seasonQuery  : 'seasons' fieldSelection? groupingClause? whereClause? ;

// --- field selection for columns ---
fieldSelection : field ((',' | WS) field)* ;
field
    : ('total' | 'avg' | 'most' | 'least' | 'count') NAME?  # aggregateField
    | NAME                                                  # nameField
    ;

// --- new group-by clause ---
groupingClause : 'by' groupingField ((',' | WS) groupingField)* # groupField ;
groupingField  : NAME ; 

// --- filters ---
whereClause  : 'where' condition ( 'and' condition )* ;
condition    : NAME operator value ;
operator     : '=' | '>' | '<' | '>=' | '<=' ;
value        : NAME | NUMBER | STRING ;

// --- tokens ---
NAME         : [a-zA-Z0-9_]+ ;
NUMBER       : [0-9]+ ;
STRING       : '\'' (~['\r\n])* '\'' ;
WS           : [ \t\r\n]+ -> skip ;