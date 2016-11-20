call AddColumnUnlessExists(Database(), 'reg_abroad', 'shareholder', 'varchar(100) DEFAULT NULL COMMENT "公司股东"');
call AddColumnUnlessExists(Database(), 'reg_internal', 'shareholder', 'varchar(100) DEFAULT NULL COMMENT "公司股东"');
call AddColumnUnlessExists(Database(), 'patent', 'date_regit', 'datetime DEFAULT NULL COMMENT "注册时间"');
