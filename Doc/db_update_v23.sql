-- 2017-11-03
call AddColumnUnlessExists(Database(), 'schedule', 'is_notify', 'int(11) DEFAULT NULL COMMENT "是否需要提醒"');
call AddColumnUnlessExists(Database(), 'schedule', 'source', 'varchar(50) DEFAULT NULL COMMENT "来源"');
call AddColumnUnlessExists(Database(), 'schedule', 'source_id', 'int(11) DEFAULT NULL COMMENT "来源ID"');
call AddColumnUnlessExists(Database(), 'schedule', 'router', 'varchar(50) DEFAULT NULL COMMENT "路由"');
call AddColumnUnlessExists(Database(), 'schedule', 'dealt_date', 'datetime DEFAULT NULL COMMENT "待办日期"');
call AddColumnUnlessExists(Database(), 'schedule', 'timeline_id', 'int(11) DEFAULT NULL');

call AddColumnUnlessExists(Database(), 'schedule', 'business_code', 'varchar(20) DEFAULT NULL COMMENT "档案号"');


call AddColumnUnlessExists(Database(), 'annual_exam', 'start_annual', 'int(11) DEFAULT NULL COMMENT "年检年份"');
