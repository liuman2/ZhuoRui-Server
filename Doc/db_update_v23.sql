-- 2017-11-03
call AddColumnUnlessExists(Database(), 'schedule', 'is_notify', 'int(11) DEFAULT NULL COMMENT "是否需要提醒"');
call AddColumnUnlessExists(Database(), 'schedule', 'source', 'varchar(50) DEFAULT NULL COMMENT "来源"');
call AddColumnUnlessExists(Database(), 'schedule', 'source_id', 'int(11) DEFAULT NULL COMMENT "来源ID"');
call AddColumnUnlessExists(Database(), 'schedule', 'router', 'varchar(50) DEFAULT NULL COMMENT "路由"');
call AddColumnUnlessExists(Database(), 'schedule', 'dealt_date', 'datetime DEFAULT NULL COMMENT "待办日期"');
call AddColumnUnlessExists(Database(), 'schedule', 'timeline_id', 'int(11) DEFAULT NULL');

call AddColumnUnlessExists(Database(), 'schedule', 'business_code', 'varchar(20) DEFAULT NULL COMMENT "档案号"');


call AddColumnUnlessExists(Database(), 'annual_exam', 'start_annual', 'int(11) DEFAULT NULL COMMENT "年检年份"');

--2017-11-26
DROP TABLE IF EXISTS `supplier`;
CREATE TABLE `supplier` (
  `id` int(11) NOT NULL,
  `name` varchar(120) DEFAULT NULL COMMENT '名称',
  `date_created` datetime DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
INSERT INTO `sequence` VALUES ('supplier', '1');

call AddColumnUnlessExists(Database(), 'reg_abroad', 'supplier_id', 'int(11) DEFAULT NULL COMMENT "供应商id"');
ALTER TABLE reg_abroad ADD CONSTRAINT fk_reg_abroad_supplier_id FOREIGN KEY (supplier_id) REFERENCES supplier(id);

call AddColumnUnlessExists(Database(), 'reg_internal', 'supplier_id', 'int(11) DEFAULT NULL COMMENT "供应商id"');
ALTER TABLE reg_internal ADD CONSTRAINT fk_reg_internal_supplier_id FOREIGN KEY (supplier_id) REFERENCES supplier(id);

call AddColumnUnlessExists(Database(), 'trademark', 'supplier_id', 'int(11) DEFAULT NULL COMMENT "供应商id"');
ALTER TABLE trademark ADD CONSTRAINT fk_trademark_supplier_id FOREIGN KEY (supplier_id) REFERENCES supplier(id);

call AddColumnUnlessExists(Database(), 'patent', 'supplier_id', 'int(11) DEFAULT NULL COMMENT "供应商id"');
ALTER TABLE patent ADD CONSTRAINT fk_patent_supplier_id FOREIGN KEY (supplier_id) REFERENCES supplier(id);

call AddColumnUnlessExists(Database(), 'audit', 'supplier_id', 'int(11) DEFAULT NULL COMMENT "供应商id"');
ALTER TABLE audit ADD CONSTRAINT fk_audit_supplier_id FOREIGN KEY (supplier_id) REFERENCES supplier(id);

call AddColumnUnlessExists(Database(), 'sub_audit', 'supplier_id', 'int(11) DEFAULT NULL COMMENT "供应商id"');
ALTER TABLE sub_audit ADD CONSTRAINT fk_sub_audit_supplier_id FOREIGN KEY (supplier_id) REFERENCES supplier(id);

call AddColumnUnlessExists(Database(), 'accounting_item', 'supplier_id', 'int(11) DEFAULT NULL COMMENT "供应商id"');
ALTER TABLE accounting_item ADD CONSTRAINT fk_accounting_item_supplier_id FOREIGN KEY (supplier_id) REFERENCES supplier(id);

-- 2017-12-03
INSERT INTO `menu` VALUES ('77', '1', 'supplier', 'fa fa-money', '供应商列表', '5');

call AddColumnUnlessExists(Database(), 'accounting', 'supplier_id', 'int(11) DEFAULT NULL COMMENT "供应商id"');
ALTER TABLE accounting ADD CONSTRAINT fk_accounting_supplier_id FOREIGN KEY (supplier_id) REFERENCES supplier(id);

-- 2017-12-04
call AddColumnUnlessExists(Database(), 'history', 'supplier_id', 'int(11) DEFAULT NULL COMMENT "供应商id"');
ALTER TABLE history ADD CONSTRAINT fk_history_supplier_id FOREIGN KEY (supplier_id) REFERENCES supplier(id);

call AddColumnUnlessExists(Database(), 'annual_exam', 'supplier_id', 'int(11) DEFAULT NULL COMMENT "供应商id"');
ALTER TABLE annual_exam ADD CONSTRAINT fk_annual_exam_supplier_id FOREIGN KEY (supplier_id) REFERENCES supplier(id);

call AddColumnUnlessExists(Database(), 'supplier', 'memo', 'varchar(300) DEFAULT NULL COMMENT "备注"');


-- 2017-12-08
call AddColumnUnlessExists(Database(), 'reg_abroad', 'annual_price', 'float(255,2) DEFAULT NULL COMMENT "年检价格"');
call AddColumnUnlessExists(Database(), 'reg_internal', 'annual_price', 'float(255,2) DEFAULT NULL COMMENT "年检价格"');
call AddColumnUnlessExists(Database(), 'trademark', 'annual_price', 'float(255,2) DEFAULT NULL COMMENT "年检价格"');
call AddColumnUnlessExists(Database(), 'patent', 'annual_price', 'float(255,2) DEFAULT NULL COMMENT "年检价格"');

-- 2017-12-13
call AddColumnUnlessExists(Database(), 'trademark', 'cut_owner', 'int(11) DEFAULT NULL COMMENT "提成所有人"');
ALTER TABLE trademark ADD CONSTRAINT fk_trademark_cut_owner FOREIGN KEY (cut_owner) REFERENCES member(id);
call AddColumnUnlessExists(Database(), 'trademark', 'annual_owner', 'int(11) DEFAULT NULL COMMENT "年检提成所有人"');
ALTER TABLE trademark ADD CONSTRAINT fk_trademark_annual_owner FOREIGN KEY (annual_owner) REFERENCES member(id);

call AddColumnUnlessExists(Database(), 'patent', 'cut_owner', 'int(11) DEFAULT NULL COMMENT "提成所有人"');
ALTER TABLE patent ADD CONSTRAINT fk_patent_cut_owner FOREIGN KEY (cut_owner) REFERENCES member(id);
call AddColumnUnlessExists(Database(), 'patent', 'annual_owner', 'int(11) DEFAULT NULL COMMENT "年检提成所有人"');
ALTER TABLE patent ADD CONSTRAINT fk_patent_annual_owner FOREIGN KEY (annual_owner) REFERENCES member(id);


-- 2017-12-20
call AddColumnUnlessExists(Database(), 'business_bank', 'branch', 'varchar(50) DEFAULT NULL COMMENT "支行"');
call AddColumnUnlessExists(Database(), 'bank_account', 'manager', 'varchar(50) DEFAULT NULL COMMENT "客户经理"');
