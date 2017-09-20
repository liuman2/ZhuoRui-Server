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


-- 2017-08-09
DROP TABLE IF EXISTS `schedule`;
CREATE TABLE `schedule` (
  `id` int(11) NOT NULL,
  `title` varchar(120) DEFAULT NULL COMMENT '主题',
  `start` datetime DEFAULT NULL COMMENT '开始时间',
  `end` datetime DEFAULT NULL COMMENT '结束时间',
  `color` varchar(50) DEFAULT NULL COMMENT '颜色',
  `type`  int(11) DEFAULT NULL COMMENT '类型0-个人, 1-部分人, 2-全公司',
  `people`  varchar(500) DEFAULT NULL COMMENT '参与人员',
  `location` varchar(120) DEFAULT NULL COMMENT '地点',
  `memo` varchar(300) DEFAULT NULL COMMENT '备注',
  `attachment` varchar(300) DEFAULT NULL COMMENT '附件',
  `created_id` int(11) NOT NULL COMMENT '创建人',
  `date_created` datetime DEFAULT CURRENT_TIMESTAMP,
  `updated_id` int(11) NOT NULL COMMENT '修改人',
  `date_updated` datetime DEFAULT NULL,

  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
INSERT INTO `sequence` VALUES ('schedule', '1');

call AddColumnUnlessExists(Database(), 'customer', 'is_delete', 'tinyint(3) DEFAULT NULL COMMENT "是否删除"');
call AddColumnUnlessExists(Database(), 'timeline', 'creator_id', 'int(11) DEFAULT NULL COMMENT "创建人"');
ALTER TABLE timeline ADD CONSTRAINT fk_timeline_creator_id FOREIGN KEY (creator_id) REFERENCES member(id);

call AddColumnUnlessExists(Database(), 'schedule', 'all_day', 'tinyint(3) DEFAULT NULL');


-- 2017-08-16
call AddColumnUnlessExists(Database(), 'schedule', 'is_repeat', 'tinyint(3) DEFAULT NULL COMMENT "是否重复"');
call AddColumnUnlessExists(Database(), 'schedule', 'repeat_type', 'tinyint(3) DEFAULT NULL COMMENT "0: 天, 1: 周, 2: 月, 3: 年"');
call AddColumnUnlessExists(Database(), 'schedule', 'repeat_dow', 'varchar(20) DEFAULT NULL COMMENT "重复的星期，值0-6(周日-周六)"');
call AddColumnUnlessExists(Database(), 'schedule', 'is_done', 'tinyint(3) DEFAULT NULL COMMENT "是否完成"');
call AddColumnUnlessExists(Database(), 'schedule', 'property', 'tinyint(3) DEFAULT NULL COMMENT "性质：0:会议, 1: 拜访客户, 2: 其它"');
call AddColumnUnlessExists(Database(), 'schedule', 'meeting_type', 'varchar(20) DEFAULT NULL COMMENT "会议类型"');
call AddColumnUnlessExists(Database(), 'schedule', 'presenter_id', 'int(11) DEFAULT NULL COMMENT "主持人"');

-- 2017-08-22
call AddColumnUnlessExists(Database(), 'customer_timeline', 'creator_id', 'int(11) DEFAULT NULL COMMENT "创建人"');
ALTER TABLE customer_timeline ADD CONSTRAINT fk_customer_timeline_creator_id FOREIGN KEY (creator_id) REFERENCES member(id);

call AddColumnUnlessExists(Database(), 'schedule', 'repeat_end', 'datetime DEFAULT NULL COMMENT "重复截止日"');


-- 2017-08-24
call AddColumnUnlessExists(Database(), 'reg_internal', 'trader_id', 'int(11) DEFAULT NULL COMMENT "渠道商"');
ALTER TABLE reg_internal ADD CONSTRAINT fk_reg_internal_trader_id FOREIGN KEY (trader_id) REFERENCES customer(id);
call AddColumnUnlessExists(Database(), 'trademark', 'trader_id', 'int(11) DEFAULT NULL COMMENT "渠道商"');
ALTER TABLE trademark ADD CONSTRAINT fk_trademark_trader_id FOREIGN KEY (trader_id) REFERENCES customer(id);
call AddColumnUnlessExists(Database(), 'patent', 'trader_id', 'int(11) DEFAULT NULL COMMENT "渠道商"');
ALTER TABLE patent ADD CONSTRAINT fk_patent_trader_id FOREIGN KEY (trader_id) REFERENCES customer(id);
call AddColumnUnlessExists(Database(), 'reg_abroad', 'date_wait', 'datetime DEFAULT NULL COMMENT "待办日期"');
call AddColumnUnlessExists(Database(), 'reg_internal', 'date_wait', 'datetime DEFAULT NULL COMMENT "待办日期"');
call AddColumnUnlessExists(Database(), 'trademark', 'date_wait', 'datetime DEFAULT NULL COMMENT "待办日期"');
call AddColumnUnlessExists(Database(), 'patent', 'date_wait', 'datetime DEFAULT NULL COMMENT "待办日期"');

-- 2017-08-28
call AddColumnUnlessExists(Database(), 'accounting_item', 'trader_id', 'int(11) DEFAULT NULL COMMENT "渠道商"');
ALTER TABLE accounting_item ADD CONSTRAINT fk_accounting_item_trader_id FOREIGN KEY (trader_id) REFERENCES customer(id);

call AddColumnUnlessExists(Database(), 'audit', 'trader_id', 'int(11) DEFAULT NULL COMMENT "渠道商"');
ALTER TABLE audit ADD CONSTRAINT fk_audit_trader_id FOREIGN KEY (trader_id) REFERENCES customer(id);

call AddColumnUnlessExists(Database(), 'sub_audit', 'trader_id', 'int(11) DEFAULT NULL COMMENT "渠道商"');
ALTER TABLE sub_audit ADD CONSTRAINT fk_sub_audit_trader_id FOREIGN KEY (trader_id) REFERENCES customer(id);

INSERT INTO `menu` VALUES ('74', '4', 'receipt_list', 'fa fa-th', '收据列表', '41');


call AddColumnUnlessExists(Database(), 'reg_abroad', 'resell_price', 'float(255,2) DEFAULT NULL COMMENT "转卖预售价"');

call AddColumnUnlessExists(Database(), 'reg_abroad', 'annual_owner', 'int(11) DEFAULT NULL COMMENT "年检提成所有人"');
ALTER TABLE reg_abroad ADD CONSTRAINT fk_reg_abroad_annual_owner FOREIGN KEY (annual_owner) REFERENCES member(id);

call AddColumnUnlessExists(Database(), 'history', 'change_owner', 'int(11) DEFAULT NULL COMMENT "变更提成所有人"');
ALTER TABLE history ADD CONSTRAINT fk_history_change_owner FOREIGN KEY (change_owner) REFERENCES member(id);

call AddColumnUnlessExists(Database(), 'history', 'area_id', 'int(11) DEFAULT NULL COMMENT "转卖归属地"');

call AddColumnUnlessExists(Database(), 'history', 'resell_id', 'int(11) DEFAULT NULL COMMENT "转卖订单id"');

call AddColumnUnlessExists(Database(), 'history', 'resell_price', 'float(255,2) DEFAULT NULL COMMENT "转卖价格"');












