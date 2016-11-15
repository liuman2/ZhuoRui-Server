call AddColumnUnlessExists(Database(), 'customer', 'assistant_id', 'int(11) DEFAULT NULL COMMENT "助理"');


call AddColumnUnlessExists(Database(), 'mail', 'order_source', 'varchar(20) DEFAULT NULL COMMENT "订单来源"');
call AddColumnUnlessExists(Database(), 'mail', 'order_id', 'int(11) DEFAULT NULL COMMENT "订单id"');
call AddColumnUnlessExists(Database(), 'mail', 'order_code', 'varchar(20) DEFAULT NULL COMMENT "订单档案号"');
call AddColumnUnlessExists(Database(), 'mail', 'order_name', 'varchar(100) DEFAULT NULL COMMENT "订单名称"');
call AddColumnUnlessExists(Database(), 'mail', 'code', 'varchar(20) DEFAULT NULL COMMENT "信件编号"');
