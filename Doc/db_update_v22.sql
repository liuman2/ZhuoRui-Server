call AddColumnUnlessExists(Database(), 'accounting_item', 'date_transaction', 'datetime DEFAULT NULL COMMENT "成交日期"');
call AddColumnUnlessExists(Database(), 'accounting_item', 'amount_transaction', 'float(255,2) DEFAULT NULL COMMENT "成交金额"');
call AddColumnUnlessExists(Database(), 'accounting_item', 'currency', 'varchar(10) DEFAULT NULL COMMENT "币别"');
call AddColumnUnlessExists(Database(), 'accounting_item', 'rate', 'float(255,2) DEFAULT NULL COMMENT "汇率"');
call AddColumnUnlessExists(Database(), 'accounting_item', 'status', 'tinyint(3) DEFAULT NULL COMMENT "订单状态 状态:0-未提交, 1-已提交, 2-财务已审核, 3-提交人已审核, 4-完成"');

call AddColumnUnlessExists(Database(), 'accounting_item', 'finance_reviewer_id', 'int(11) DEFAULT NULL COMMENT "财务审核人员ID"');
ALTER TABLE accounting_item ADD CONSTRAINT fk_accounting_item_finance_reviewer_id FOREIGN KEY (finance_reviewer_id) REFERENCES member(id);
call AddColumnUnlessExists(Database(), 'accounting_item', 'finance_review_date', 'datetime DEFAULT NULL COMMENT "财务审核日期"');
call AddColumnUnlessExists(Database(), 'accounting_item', 'finance_review_moment', 'varchar(100) DEFAULT NULL COMMENT "财务审核意见"');


call AddColumnUnlessExists(Database(), 'accounting_item', 'submit_reviewer_id', 'int(11) DEFAULT NULL COMMENT "提交审核人员ID"');
ALTER TABLE accounting_item ADD CONSTRAINT fk_accounting_item_submit_reviewer_id FOREIGN KEY (submit_reviewer_id) REFERENCES member(id);
call AddColumnUnlessExists(Database(), 'accounting_item', 'submit_review_date', 'datetime DEFAULT NULL COMMENT "提交审核日期"');
call AddColumnUnlessExists(Database(), 'accounting_item', 'submit_review_moment', 'varchar(100) DEFAULT NULL COMMENT "提交审核意见"');

call AddColumnUnlessExists(Database(), 'accounting_item', 'review_status', 'int(11) DEFAULT NULL COMMENT "审核状体 未审核：-1；未通过：0；已通过：1"');

call AddColumnUnlessExists(Database(), 'accounting_item', 'creator_id', 'int(11) DEFAULT NULL COMMENT "创建者"');
ALTER TABLE accounting_item ADD CONSTRAINT fk_accounting_item_creator_id FOREIGN KEY (creator_id) REFERENCES member(id);

call AddColumnUnlessExists(Database(), 'accounting_item', 'salesman_id', 'int(11) DEFAULT NULL COMMENT "业务员"');
ALTER TABLE accounting_item ADD CONSTRAINT fk_accounting_item_salesman_id FOREIGN KEY (salesman_id) REFERENCES member(id);

call AddColumnUnlessExists(Database(), 'accounting_item', 'accountant_id', 'int(11) DEFAULT NULL COMMENT "业务员"');
ALTER TABLE accounting_item ADD CONSTRAINT fk_accounting_item_accountant_id FOREIGN KEY (accountant_id) REFERENCES member(id);

call AddColumnUnlessExists(Database(), 'accounting_item', 'manager_id', 'int(11) DEFAULT NULL COMMENT "经理"');
ALTER TABLE accounting_item ADD CONSTRAINT fk_accounting_item_manager_id FOREIGN KEY (manager_id) REFERENCES member(id);

call AddColumnUnlessExists(Database(), 'accounting_item', 'assistant_id', 'int(11) DEFAULT NULL COMMENT "助理"');
ALTER TABLE accounting_item ADD CONSTRAINT fk_accounting_item_assistant_id FOREIGN KEY (assistant_id) REFERENCES member(id);

call AddColumnUnlessExists(Database(), 'accounting_item', 'tax', 'tinyint(3) DEFAULT NULL COMMENT "是否含税"');

call AddColumnUnlessExists(Database(), 'accounting_item', 'invoice_name', 'varchar(200) DEFAULT NULL COMMENT "开票信息名称"');
call AddColumnUnlessExists(Database(), 'accounting_item', 'invoice_tax', 'varchar(200) DEFAULT NULL COMMENT "纳税人识别号"');
call AddColumnUnlessExists(Database(), 'accounting_item', 'invoice_address', 'varchar(200) DEFAULT NULL COMMENT "开票信息地址"');
call AddColumnUnlessExists(Database(), 'accounting_item', 'invoice_tel', 'varchar(20) DEFAULT NULL COMMENT "开票信息电话"');
call AddColumnUnlessExists(Database(), 'accounting_item', 'invoice_bank', 'varchar(100) DEFAULT NULL COMMENT "开票信息开户行"');
call AddColumnUnlessExists(Database(), 'accounting_item', 'invoice_account', 'varchar(100) DEFAULT NULL COMMENT "开票信息开户行账号"');

call AddColumnUnlessExists(Database(), 'accounting', 'pay_mode', 'tinyint(3) DEFAULT NULL COMMENT "付款方式: 1-月付,3-季付,6-半年付,12-一年付"');
call AddColumnUnlessExists(Database(), 'accounting', 'pay_notify', 'datetime DEFAULT NULL COMMENT "下次催收时间"');

call AddColumnUnlessExists(Database(), 'accounting_item', 'pay_mode', 'tinyint(3) DEFAULT NULL COMMENT "付款方式: 1-月付,3-季付,6-半年付,12-一年付"');

-- 2017-07-10
call AddColumnUnlessExists(Database(), 'timeline', 'log_type', 'tinyint(3) DEFAULT NULL COMMENT "日志类型: 1-联系客户, 9-其它"');
call AddColumnUnlessExists(Database(), 'reg_abroad', 'date_last', 'datetime DEFAULT NULL COMMENT "最近联系客户日期"');
call AddColumnUnlessExists(Database(), 'reg_abroad', 'title_last', 'varchar(100) DEFAULT NULL COMMENT "最近联系内容"');
call AddColumnUnlessExists(Database(), 'reg_internal', 'date_last', 'datetime DEFAULT NULL COMMENT "最近联系客户日期"');
call AddColumnUnlessExists(Database(), 'reg_internal', 'title_last', 'varchar(100) DEFAULT NULL COMMENT "最近联系内容"');
call AddColumnUnlessExists(Database(), 'trademark', 'date_last', 'datetime DEFAULT NULL COMMENT "最近联系客户日期"');
call AddColumnUnlessExists(Database(), 'trademark', 'title_last', 'varchar(100) DEFAULT NULL COMMENT "最近联系内容"');
call AddColumnUnlessExists(Database(), 'patent', 'date_last', 'datetime DEFAULT NULL COMMENT "最近联系客户日期"');
call AddColumnUnlessExists(Database(), 'patent', 'title_last', 'varchar(100) DEFAULT NULL COMMENT "最近联系内容"');

-- 2017-07-17
call AddColumnUnlessExists(Database(), 'reg_abroad', 'trader_id', 'int(11) DEFAULT NULL COMMENT "渠道商"');
ALTER TABLE reg_abroad ADD CONSTRAINT fk_reg_abroad_trader_id FOREIGN KEY (trader_id) REFERENCES customer(id);

-- 2017-07-18
call AddColumnUnlessExists(Database(), 'reg_abroad', 'annual_id', 'int(11) DEFAULT NULL COMMENT "年检id"');


-- 2017-07-19
call AddColumnUnlessExists(Database(), 'mail', 'province', 'varchar(20) DEFAULT NULL COMMENT "邮寄地址省份"');
call AddColumnUnlessExists(Database(), 'mail', 'city', 'varchar(20) DEFAULT NULL COMMENT "邮寄地址市"');
call AddColumnUnlessExists(Database(), 'mail', 'county', 'varchar(20) DEFAULT NULL COMMENT "邮寄地址县"');


-- 2017-07-27
call AddColumnUnlessExists(Database(), 'lecture_customer', 'contact_id', 'int(11) DEFAULT NULL COMMENT "联系人id"');
ALTER TABLE lecture_customer ADD CONSTRAINT fk_lecture_customer_contact_id FOREIGN KEY (contact_id) REFERENCES contact(id);


-- 2017-08-01
INSERT INTO `menu` VALUES ('73', '4', 'logoff_order', 'fa fa-th', '免年检订单列表', '40');


-- 2017-08-03
INSERT INTO `operation` VALUES ('6', '数据导出');

