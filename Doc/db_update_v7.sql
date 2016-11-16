call AddColumnUnlessExists(Database(), 'mail', 'receiver', 'varchar(30) DEFAULT NULL COMMENT "收件人"');
call AddColumnUnlessExists(Database(), 'mail', 'tel', 'varchar(20) DEFAULT NULL COMMENT "电话"');

call AddColumnUnlessExists(Database(), 'mail', 'review_status', 'tinyint(3) DEFAULT NULL COMMENT "未审核：-1；未通过：0；已通过：1"');
call AddColumnUnlessExists(Database(), 'mail', 'review_date', 'datetime DEFAULT NULL COMMENT "审核日期"');
call AddColumnUnlessExists(Database(), 'mail', 'review_moment', 'varchar(50) DEFAULT NULL COMMENT "审核意见"');
