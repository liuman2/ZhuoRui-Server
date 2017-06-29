call AddColumnUnlessExists(Database(), 'reg_abroad', 'order_status', 'int(11) DEFAULT NULL COMMENT "0 正常 1 转出 2 注销"');
call AddColumnUnlessExists(Database(), 'reg_internal', 'order_status', 'int(11) DEFAULT NULL COMMENT "0 正常 1 转出 2 注销"');
call AddColumnUnlessExists(Database(), 'trademark', 'order_status', 'int(11) DEFAULT NULL COMMENT "0 正常 1 转出 2 注销"');
call AddColumnUnlessExists(Database(), 'patent', 'order_status', 'int(11) DEFAULT NULL COMMENT "0 正常 1 转出 2 注销"');

-- update reg_abroad set order_status = 0;
-- update reg_internal set order_status = 0;
-- update trademark set order_status = 0;
-- update patent set order_status = 0;
