call AddColumnUnlessExists(Database(), 'reg_internal', 'names', 'text DEFAULT NULL COMMENT "公司备选名称"');
call AddColumnUnlessExists(Database(), 'reg_internal', 'prices', 'text DEFAULT NULL COMMENT "费用记录"');
call AddColumnUnlessExists(Database(), 'reg_internal', 'shareholders', 'text DEFAULT NULL COMMENT "股东"');
call AddColumnUnlessExists(Database(), 'reg_internal', 'card_no', 'varchar(100) DEFAULT NULL COMMENT "身份证号"');
call AddColumnUnlessExists(Database(), 'reg_internal', 'scope', 'text DEFAULT NULL COMMENT "经营范围"');
call AddColumnUnlessExists(Database(), 'reg_internal', 'pay_mode', 'varchar(10) DEFAULT NULL COMMENT "缴交方式"');
