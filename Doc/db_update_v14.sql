call AddColumnUnlessExists(Database(), 'timeline', 'date_business', 'datetime DEFAULT NULL COMMENT "发生时间"');
call AddColumnUnlessExists(Database(), 'timeline', 'is_system', 'tinyint(3) DEFAULT NULL COMMENT "是否系统日志"');
