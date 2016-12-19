call AddColumnUnlessExists(Database(), 'trademark', 'date_allege', 'datetime DEFAULT NULL COMMENT "异议时间"');
call AddColumnUnlessExists(Database(), 'trademark', 'trial_type', 'varchar(10) DEFAULT NULL COMMENT "初审类型"');
call AddColumnUnlessExists(Database(), 'trademark', 'exten_period', 'int(11) DEFAULT NULL COMMENT "续展周期"');
call AddColumnUnlessExists(Database(), 'trademark', 'regit_no', 'varchar(100) DEFAULT NULL COMMENT "注册号"');


