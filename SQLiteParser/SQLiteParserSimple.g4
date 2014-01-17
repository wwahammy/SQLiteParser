grammar SQLiteParserSimple;

options {
    language=CSharp3;
    TokenLabelType=CommonToken;
  //  output=AST;
   // ASTLabelType=CommonTree;
}

@lexer::members{
public const int EOF = Eof;
}

@parser::members{
public const int EOF = Eof;
}

/*
 * Parser Rules
 */

/*sql_stmt_list: ((sql_stmt (sql_stmt SEMICOLON)*) | (sql_stmt SEMICOLON) ) EOF;*/
sql_stmt : (create_table_stmt |create_index_stmt) SEMICOLON? EOF;

create_index_stmt: CREATE UNIQUE? INDEX (IF NOT EXISTS)? (database_name PERIOD)? index_name ON table_name LP indexed_column (COMMA indexed_column)* RP (WHERE expr)?;
	
create_table_stmt : CREATE TEMPORARY? TABLE (IF NOT EXISTS)? (database_name PERIOD)? table_name (LP column_def (COMMA column_def)*  (COMMA table_constraint)* RP  (WITHOUT ROWid)?) | (AS select_stmt) ;

column_def:  name type_name? column_constraint*;

type_name : id (LP signed_number (COMMA signed_number)* RP) ?;

signed_number: (PLUS|MINUS)? NUMERIC_LITERAL;





column_constraint : (CONSTRAINT id)? column_constraint__postfix;

column_constraint__postfix : NOT NULL conflict_clause
    {
      
    }
    |
    PRIMARY KEY (ASC | DESC)? conflict_clause (AUTOINCREMENT)?
    {
       
    }
    |
    UNIQUE conflict_clause
    {
       
    }  
    |
    CHECK_C LP expr RP
    {
        
    }
    |
    DEFAULT ( signed_number | literal_value | ( LP expr RP ))
    {
        
    }
    |
	COLLATE collation_name
	|NULL
	| foreign_key_clause;


foreign_key_clause: REFERENCES table_name foreign_key_clause__parens_field_list? (foreign_key_clause__on_delete | foreign_key_clause__on_update | foreign_key_clause__match)* foreign_key_clause__deferrable?;





foreign_key_clause__parens_field_list : LP foreign_key_clause__column_list RP;
foreign_key_clause__column_list : id (COMMA id)*;



foreign_key_clause__deferrable: NOT? DEFERRABLE ((INITIALLY DEFERRED)| (INITIALLY IMMEDIATE))?;
foreign_key_clause__on_delete: ON DELETE (SET NULL|SET DEFAULT|CASCADE|RESTRICT|NO ACTION);
foreign_key_clause__on_update: ON UPDATE (SET NULL|SET DEFAULT|CASCADE|RESTRICT|NO ACTION);
foreign_key_clause__match: MATCH id;

conflict_clause: (ON CONFLICT (ROLLBACK| ABORT|FAIL|IGNORE|REPLACE))?;


table_constraint: (CONSTRAINT name)? (table_constraint__index_clause | table_constraint__check | table_constraint__foreign_key_constraint);


table_constraint__index_clause : ((PRIMARY KEY) | UNIQUE) table_constraint__indexed_columns conflict_clause;

table_constraint__indexed_columns: LP table_constraint__indexed_column (COMMA table_constraint__indexed_column)? RP;
table_constraint__indexed_column: id (COLLATE id)? (ASC | DESC)?;

table_constraint__foreign_key_constraint: FOREIGN KEY table_constraint__parens_field_list foreign_key_clause;

table_constraint__parens_field_list : LP foreign_key_clause__column_list RP;
table_constraint__column_list : id (COMMA id)*;

table_constraint__check: CHECK LP expr RP;

indexed_column: id (COLLATE id)? (ASC | DESC)?;

collation_name: id;


//holy cow
expr: literal_value
	| bind_parameter
	| ((database_name PERIOD)? table_name PERIOD)? column_name
	| unary_operator expr
	| expr binary_operator expr
	| expr__function_expr
	| LP expr RP
	| CAST LP expr AS type_name RP //?
	| expr COLLATE name
	| expr NOT? (LIKE |GLOB| REGEXP | MATCH) expr (ESCAPE expr)?
	| expr ((IS NULL) | ( NOT NULL))
	| expr IS NOT? expr
	| expr NOT? BETWEEN expr AND expr
	| expr NOT? IN 
	(table_name |
	 (
	 LP (
			select_stmt | 
							(
								expr 
									(
										COMMA expr
									)? 
							)
							)? RP))
	| (NOT? EXISTS)? LP select_stmt RP
	| CASE expr?  (WHEN expr THEN expr)+ (ELSE expr)? END
	| raise_function;


select_stmt: select_core select_stmt__compound_operator_core* (ORDER BY  ordering_term (COMMA ordering_term)*)? (LIMIT expr ((OFFSET |COMMA) expr)?)?;
select_stmt__compound_operator_core: compound_operator select_core;

select_core: SELECT (DISTINCT|ALL)? result_column (COMMA result_column)* (FROM join_source)? (WHERE expr)? select_core__group_by?;

result_column: STAR | (table_name PERIOD STAR) | (expr (AS? column_alias));

join_source: single_source (join_op single_source join_constraint)*;

single_source: ((database_name PERIOD)? table_name ( AS? table_alias)? ((INDEXED BY index_name)|(NOT INDEXED))?)| (LP select_stmt RP (AS? table_alias)?) | (LP join_source RP);

join_op: (COMMA) | ( NATURAL? ((LEFT OUTER?)| INNER | OUTER )? JOIN);

join_constraint: ((ON expr)|(USING LP column_name (COMMA column_name)* RP))?;
ordering_term: expr (COLLATE collation_name)? (ASC | DESC)?;
compound_operator: (UNION ALL?)| INTERSECT | EXCEPT;
update_stmt: UPDATE (OR ROLLBACK|ABORT|FAIL|REPLACE|IGNORE)? qualified_table_name SET column_name EQ expr (COMMA column_name EQ expr)* (WHERE expr)? update_stmt__limit_ending?;
update_stmt__limit_ending: (ORDER BY ordering_term (COMMA ordering_term))? LIMIT expr ((OFFSET| COMMA) expr)?;
qualified_table_name: (database_name PERIOD)? table_name ((INDEXED BY id)| (NOT INDEXED))?;

vacuum_stmt: VACUUM;


select_core__group_by: GROUP BY expr (COMMA expr)* select_core__group_by_having?;
select_core__group_by_having: HAVING expr;

expr__function_expr: function_name LP expr__function_expr_params RP;
expr__function_expr_params: ( STAR | (DISTINCT? expr (COMMA expr)*))?;

raise_function: RAISE LP (IGNORE | ((ROLLBACK | ABORT | FAIL) COMMA error_message)) RP;


bind_parameter: '?' | ('?' INT) | (':' id) | ('@' id) | ('$' id);
literal_value: NUMERIC_LITERAL
		| STRING_LITERAL
		| BLOB_LITERAL
		| NULL
		| CURRENT_TIME
		| CURRENT_DATE
		| CURRENT_TIMESTAMP;


binary_operator: '||'| STAR | '/' | '%' | PLUS |MINUS| '<<' | '>>' | '&' | '|' | '<' | '<=' | '>' | '>=' | '=' |'==' | '!=' | '<>' | IS | (IS NOT) | IN | LIKE |GLOB| MATCH |REGEXP | AND |OR;
unary_operator: MINUS | PLUS | '~' | NOT;
function_name: id;
error_message: STRING_LITERAL;
name: QUOTE? id QUOTE?;
column_alias: id;
column_name: id;
database_name: id; 
table_name: id;
index_name: id;
table_alias: id;

id: WITHOUT|WHERE|WHEN |VIEW |VALUE|VACUUM|USING|UPDATE|UNIQUE |UNION|TRIGGER |TRANSACTION|THEN|TEMPORARY |TABLE |SET|SELECT |ROWID|ROW|ROLLBACK|RESTRICT|REPLACE|REGEXP|REFERENCES |RAISE|PRIMARY|OUTER|ORDER|OR|ON |OFFSET|OF|NULL |NOT_NULL |NOT|NO|NATURAL|MATCH|LIMIT|LIKE|LEFT|KEY|JOIN|IS|INTERSECT|INSTEAD|INSERT|INNER|INITIALLY|INDEXED|INDEX |IN|IMMEDIATE|IGNORE|IF |HAVING|GROUP|GLOB|FROM|FOREIGN_KEY |FOREIGN|FOR|FAIL|EXISTS|EXCEPT|ESCAPE|END_C |END|ELSE|EACH|DROP |DISTINCT|DESC|DELETE|DEFERRED|DEFERRABLE|DEFAULT |CURRENT_TIMESTAMP |CURRENT_TIME|CURRENT_DATE|CREATE |CONSTRAINT |CONFLICT|COMMIT|COLLATE|CHECK_C |CHECK|CAST|CASE|CASCADE|BY|BETWEEN|BEGIN_C |BEFORE|AUTOINCREMENT |ASC|AS |AND|ALL|AFTER|ACTION|ABORT | ID;

// START: tokens


STRING_LITERAL: '\'' (~'\'')* '\'';
NUMERIC_LITERAL: ((INT (PERIOD INT*)?)| (PERIOD INT)) (('e'|'E') (PLUS|MINUS)? INT)?;
BLOB_LITERAL: X STRING_LITERAL;


NULL : N U L L;
CURRENT_TIMESTAMP : C U R R E N T '_' T I M E S T A M P;
CURRENT_TIME: C U R R E N T '_' T I M E;
CURRENT_DATE: C U R R E N T '_' D A T E;
COMMIT: C O M M I T;
INSERT: I N S E R T;
UPDATE: U P D A T E;
ASC: A S C;
DESC: D E S C;
INSTEAD: I N S T E A D;
OF: O F;
PRIMARY: P R I M A R Y;
FOREIGN: F O R E I G N;
KEY: K E Y;
VALUE: V A L U E;
MATCH: M A T C H;
GLOB: G L O B;
CAST: C A S T;
LIKE: L I K E;
REGEXP: R E G E X P;
ESCAPE: E S C A P E;


IS: I S;
BETWEEN: B E T W E E N;
AND: A N D;
IN: I N;
CASE: C A S E;
THEN: T H E N;
ELSE: E L S E;
END: E N D;
DISTINCT: D I S T I N C T;
RAISE: R A I S E;
OR: O R;
CHECK: C H E C K;

CREATE : C R E A T E;

BEGIN_C : B E G I N;

END_C : E N D ;

TRANSACTION: T R A N S A C T I O N;




TEMPORARY : T E M P (O R A R Y)? ;

TABLE : T A B L E;

INDEX : I N D E X;

NOT_NULL : N O T N U L L;



FOREIGN_KEY : F O R E I G N K E Y;

CHECK_C : C H E C K;

DEFAULT : D E F A U L T;

TRIGGER : T R I G G E R;

VIEW : V I E W;

SELECT : S E L E C T;

DROP : D R O P;
ON : O N;

AS : A S;

WHEN : W H E N;
DEFERRED: D E F E R R E D;
DEFERRABLE: D E F E R R A B L E;

REFERENCES : R E F E R E N C E S;

CONSTRAINT : C O N S T R A I N T ;

AUTOINCREMENT : A U T O I N C R E M E N T;
DELETE: D E L E T E;
SET: S E T;
CASCADE: C A S C A D E;
RESTRICT: R E S T R I C T;
NO: N O;
ACTION: A C T I O N;
CONFLICT: C O N F L I C T;
ROLLBACK: R O L L B A C K;
ABORT: A B O R T;
FAIL: F A I L;
IGNORE: I G N O R E;
REPLACE: R E P L A C E;
FOR: F O R;
EACH: E A C H;
ROW: R O W;
BEFORE: B E F O R E;
AFTER: A F T E R;
NOT: N O T;
WITHOUT: W I T H O U T;
ROWID: R O W I D;
COLLATE: C O L L A T E;
INITIALLY: I N I T I A L L Y;
IMMEDIATE: I M M E D I A T E;

UNIQUE : U N I Q U E;
ALL: A L L;
BY: B Y;

EXCEPT: E X C E P T;
FROM: F R O M;
GROUP: G R O U P;
HAVING: H A V I N G;
INDEXED: I N D E X E D;
INNER: I N N E R;
INTERSECT: I N T E R S E C T;
JOIN: J O I N;
LEFT: L E F T;
LIMIT: L I M I T;
NATURAL: N A T U R A L;
OFFSET: O F F S E T;
ORDER: O R D E R;
OUTER: O U T E R;
UNION: U N I O N;
USING: U S I N G;
WHERE: W H E R E;
VACUUM: V A C U U M;
IF : I F;
EXISTS: E X I S T S;
EQ: '=';
SEMICOLON : ';';




LP : '(';

RP: ')';
STAR: '*';

QUOTE : '\''|'"';


COMMA: ',';
INT: DIGIT+;


PLUS            : '+' ;
MINUS           : '-' ;
PERIOD          : '.';

ID: LETTER (LETTER|DIGIT)*;
fragment LETTER : [a-zA-Z_];

fragment DIGIT: '0'..'9';
fragment ANYCHAR : . ; 

fragment A:('a'|'A');
fragment B:('b'|'B');
fragment C:('c'|'C');
fragment D:('d'|'D');
fragment E:('e'|'E');
fragment F:('f'|'F');
fragment G:('g'|'G');
fragment H:('h'|'H');
fragment I:('i'|'I');
fragment J:('j'|'J');
fragment K:('k'|'K');
fragment L:('l'|'L');
fragment M:('m'|'M');
fragment N:('n'|'N');
fragment O:('o'|'O');
fragment P:('p'|'P');
fragment Q:('q'|'Q');
fragment R:('r'|'R');
fragment S:('s'|'S');
fragment T:('t'|'T');
fragment U:('u'|'U');
fragment V:('v'|'V');
fragment W:('w'|'W');
fragment X:('x'|'X');
fragment Y:('y'|'Y');
fragment Z:('z'|'Z');

WS : (' ' | '\t' | '\r'| '\n')+ -> skip;

//END: tokens
