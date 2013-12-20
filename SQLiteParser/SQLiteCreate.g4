/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

grammar SQLiteCreate;

//START: rules

statement : begin_transaction
    | commit
    | drop;
//    | comment
 //   | create
    //| EOF;

begin_transaction : BEGIN_C TRANSACTION? SEMICOLON;

commit : COMMIT SEMICOLON;

drop : DROP (tbl_drop | view_drop | trg_drop) SEMICOLON;

tbl_drop: TABLE COMMIT table_name;

view_drop: VIEW if_exists? view_name;

trg_drop: TRIGGER if_exists? trigger_name;


table_name : qualified_name;

qualified_name : name
               |(WORD)'.' (WORD);

field_name : name;

constraint_name : name;

view_name : qualified_name;

trigger_name : qualified_name;

if_exists : IF EXISTS;

name: QUOTE? (WORD) QUOTE?;


value : ('-'|'+')? '.'? DIGIT+ (('e'|'E')DIGIT+)?
        
          {  }

    | '\'' ANYCHAR? '\''

    | NULL
  
    | CURRENT_TIMESTAMP
   
  ;

//END: rules



  

// START: tokens


BEGIN_C : B E G I N;

END_C : E N D ;

TRANSACTION: T R A N S A C T I O N;

CREATE : C R E A T E;

TEMPORARY : T E M P (O R A R Y)? ;

TABLE : T A B L E;

INDEX : I N D E X;

NOT_NULL : N O T N U L L;

PRIMARY_KEY : P R I M A R Y K E Y;

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

REFERENCES : R E F E R E N C E S;

CONSTRAINT : C O N S T R A I N T ;

AUTOINCREMENT : A U T O I N C R E M E N T;

UNIQUE : 'unique';

SEMICOLON : ';';
WORD : [a-zA-Z_0-9]+;

IF : I F;
EXISTS: E X I S T S;

DIGIT: [0-9];
ANYCHAR : . ; 


QUOTE : '\''|'"';
NULL : N U L L;
CURRENT_TIMESTAMP : C U R R E N T '_' T I M E S T A M P;
COMMIT: C O M M I T;


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

WS : (' ' | '\t' | '\r'| '\n')+ -> skip ;

//END: tokens











/////MORE STUFF!!!!

create : CREATE TEMPORARY? UNIQUE? INDEX NAME ON table_name parens_field_list conflict_clause? SEMICOLON
         
          {
        my $db_name    = $item[7]->{'db_name'} || '';
        my $table_name = $item[7]->{'name'};

        my $index        =  {
            name         => $item[5],
            fields       => $item[8],
            on_conflict  => $item[9][0],
            is_temporary => $item[2][0] ? 1 : 0,
        };

        my $is_unique = $item[3][0];

        if ( $is_unique ) {
            $index->{'type'} = 'unique';
            push @{ $tables{ $table_name }{'constraints'} }, $index;
        }
        else {
            push @{ $tables{ $table_name }{'indices'} }, $index;
        }
    }

    | CREATE TEMPORARY? TABLE table_name '(' definition(s /,/) ')' SEMICOLON
    {
        my $db_name    = $item[4]->{'db_name'} || '';
        my $table_name = $item[4]->{'name'};

        $tables{ $table_name }{'name'}         = $table_name;
        $tables{ $table_name }{'is_temporary'} = $item[2][0] ? 1 : 0;
        $tables{ $table_name }{'order'}        = ++$table_order;

        for my $def ( @{ $item[6] } ) {
            if ( $def->{'supertype'} eq 'column' ) {
                push @{ $tables{ $table_name }{'fields'} }, $def;
            }
            elsif ( $def->{'supertype'} eq 'constraint' ) {
                push @{ $tables{ $table_name }{'constraints'} }, $def;
            }
        }
    }
    |CREATE TEMPORARY? TRIGGER NAME before_or_after? database_event ON table_name trigger_action SEMICOLON
    {
        my $table_name = $item[8]->{'name'};
        push @triggers, {
            name         => $item[4],
            is_temporary => $item[2][0] ? 1 : 0,
            when         => $item[5][0],
            instead_of   => 0,
            db_events    => [ $item[6] ],
            action       => $item[9],
            on_table     => $table_name,
        }
    }

    | CREATE TEMPORARY? TRIGGER NAME instead_of database_event ON view_name trigger_action
    {
        my $table_name = $item[8]->{'name'};
        push @triggers, {
            name         => $item[4],
            is_temporary => $item[2][0] ? 1 : 0,
            when         => undef,
            instead_of   => 1,
            db_events    => [ $item[6] ],
            action       => $item[9],
            on_table     => $table_name,
        }
    }
| CREATE TEMPORARY(?) VIEW view_name AS select_statement
    {
        push @views, {
            name         => $item[4]->{'name'},
            sql          => $item[6],
            is_temporary => $item[2][0] ? 1 : 0,
        }
    };
    
    
definition : constraint_def | column_def;

column_def: comment* NAME type? column_constraint_def*
    {
        my $column = {
            supertype      => 'column',
            name           => $item[2],
            data_type      => $item[3][0]->{'type'},
            size           => $item[3][0]->{'size'},
            is_nullable    => 1,
            is_primary_key => 0,
            is_unique      => 0,
            check          => '',
            default        => undef,
            constraints    => $item[4],
            comments       => $item[1],
        };


        for my $c ( @{ $item[4] } ) {
            if ( $c->{'type'} eq 'not_null' ) {
                $column->{'is_nullable'} = 0;
            }
            elsif ( $c->{'type'} eq 'primary_key' ) {
                $column->{'is_primary_key'} = 1;
            }
            elsif ( $c->{'type'} eq 'unique' ) {
                $column->{'is_unique'} = 1;
            }
            elsif ( $c->{'type'} eq 'check' ) {
                $column->{'check'} = $c->{'expression'};
            }
            elsif ( $c->{'type'} eq 'default' ) {
                $column->{'default'} = $c->{'value'};
            }
            elsif ( $c->{'type'} eq 'autoincrement' ) {
                $column->{'is_auto_inc'} = 1;
            }
        }

        $column;
    };

type : WORD parens_value_list?
    {
        $return = {
            type => $item[1],
            size => $item[2][0],
        }
    };

column_constraint_def : CONSTRAINT constraint_name column_constraint
    {
        $return = {
            name => $item[2],
            %{ $item[3] },
        }
    }
    |
    column_constraint;

column_constraint : NOT_NULL conflict_clause?
    {
        $return = {
            type => 'not_null',
        }
    }
    |
    PRIMARY_KEY sort_order? conflict_clause?
    {
        $return = {
            type        => 'primary_key',
            sort_order  => $item[2][0],
            on_conflict => $item[2][0],
        }
    }
    |
    UNIQUE conflict_clause?
    {
        $return = {
            type        => 'unique',
            on_conflict => $item[2][0],
        }
    }
    |
    CHECK_C '(' expr ')' conflict_clause?
    {
        $return = {
            type        => 'check',
            expression  => $item[3],
            on_conflict => $item[5][0],
        }
    }
    |
    DEFAULT VALUE
    {
        $return   = {
            type  => 'default',
            value => $item[2],
        }
    }
    |
    REFERENCES ref_def cascade_def?
    {
        $return   = {
            type             => 'foreign_key',
            reference_table  => $item[2]{'reference_table'},
            reference_fields => $item[2]{'reference_fields'},
            on_delete        => $item[3][0]{'on_delete'},
            on_update        => $item[3][0]{'on_update'},
        }
    }
    |
    AUTOINCREMENT
    {
        $return = {
            type => 'autoincrement',
        }
    };

constraint_def : comment(s?) CONSTRAINT constraint_name table_constraint
    {
        $return = {
            comments => $item[1],
            name => $item[3],
            %{ $item[4] },
        }
    }
    |
    comment(s?) table_constraint
    {
        $return = {
            comments => $item[1],
            %{ $item[2] },
        }
    };

table_constraint : PRIMARY_KEY parens_field_list conflict_clause?
    {
        $return         = {
            supertype   => 'constraint',
            type        => 'primary_key',
            fields      => $item[2],
            on_conflict => $item[3][0],
        }
    }
    |
    UNIQUE parens_field_list conflict_clause?
    {
        $return         = {
            supertype   => 'constraint',
            type        => 'unique',
            fields      => $item[2],
            on_conflict => $item[3][0],
        }
    }
    |
    CHECK_C '(' expr ')' conflict_clause?
    {
        $return         = {
            supertype   => 'constraint',
            type        => 'check',
            expression  => $item[3],
            on_conflict => $item[5][0],
        }
    }
    |
    FOREIGN_KEY parens_field_list REFERENCES ref_def cascade_def?
    {
      $return = {
        supertype        => 'constraint',
        type             => 'foreign_key',
        fields           => $item[2],
        reference_table  => $item[4]{'reference_table'},
        reference_fields => $item[4]{'reference_fields'},
        on_delete        => $item[5][0]{'on_delete'},
        on_update        => $item[5][0]{'on_update'},
      }
    };



ref_def : table_name parens_field_list
    { $return = { reference_table => $item[1]{name}, reference_fields => $item[2] } };

cascade_def : cascade_update_def cascade_delete_def?
    { $return = {  on_update => $item[1], on_delete => $item[2][0] } }
    |
    cascade_delete_def cascade_update_def?
    { $return = {  on_delete => $item[1], on_update => $item[2][0] } };

cascade_delete_def : on delete (set null|set default|cascade|restrict|no action)
    { $return = $1};

cascade_update_def : on update (set null|set default|cascade|restrict|no action)
    { $return = $1};

table_name : qualified_name;

qualified_name : NAME
    { $return = { name => $item[1] } };

qualified_name : (WORD+)'.' (WORD+)
    { $return = { db_name => $1, name => $2 } };

field_name : NAME;

constraint_name : NAME;

conflict_clause : on conflict conflict_algorigthm;

conflict_algorigthm : (rollback|abort|fail|ignore|replace);

parens_field_list : '(' column_list ')'
    { $item[2] };

column_list : field_name(s /,/);

parens_value_list : '(' VALUE(s /,/) ')'
    { $item[2] };

expr : [^)]+;

sort_order : (ASC|DESC);

database_event : /(delete|insert|update)/i

database_event : /update of/i column_list

trigger_action : for_each? when? BEGIN_C trigger_step(s) END_C
    {
        $return = {
            for_each => $item[1][0],
            when     => $item[2][0],
            steps    => $item[4],
        }
    };

for_each : FOR EACH ROW;

when : WHEN expr { $item[2] };

string :
   (\.|''|[^\'])*;

nonstring : [^;\'\"]+;

statement_body : string | nonstring;

trigger_step : (select|delete|insert|update) statement_body(s?) SEMICOLON
    {
        $return = join( ' ', $item[1], join ' ', @{ $item[2] || [] } )
    };

before_or_after : (before|after) { $return = lc $1 }

instead_of : instead of;

if_exists : if exists;

view_name : qualified_name;

trigger_name : qualified_name;

select_statement : SELECT [^;]+ SEMICOLON
    {
        $return = join( ' ', $item[1], $item[2] );
    };
