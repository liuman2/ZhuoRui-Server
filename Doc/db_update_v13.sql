DROP TABLE IF EXISTS `sub_audit`;
CREATE TABLE `sub_audit` (
  `id` int(11) NOT NULL,
  `master_id` int(11) NOT NULL,
  `customer_id` int(11) NULL,
  `account_period` datetime DEFAULT NULL COMMENT '账期',
  `account_period2` datetime DEFAULT NULL COMMENT '账期',
  `date_year_end` varchar(6) DEFAULT NULL,
  `turnover` float(255,2) DEFAULT NULL COMMENT '营业额',
  `amount_bank` float(255,2) DEFAULT NULL COMMENT '银行入账金额',
  `bill_number` int(11) DEFAULT NULL COMMENT '单据量',
  `accounting_standard` varchar(50) DEFAULT NULL COMMENT '会计准则',
  `cost_accounting` float(255,2) DEFAULT NULL COMMENT '做账费用',
  `date_transaction` datetime DEFAULT NULL COMMENT '成交日期',
  `amount_transaction` float(255,2) DEFAULT NULL COMMENT '成交金额',
  `currency` varchar(10) DEFAULT NULL COMMENT '币别',
  `rate` float(255,2) DEFAULT NULL COMMENT '汇率',
  `progress` varchar(50) DEFAULT NULL COMMENT '审计进度',
  `status` tinyint(3) DEFAULT NULL COMMENT '订单状态 状态:0-未提交, 1-已提交, 2-财务已审核, 3-提交人已审核, 4-完成',
  `finance_reviewer_id` int(11) DEFAULT NULL COMMENT '财务审核人员ID',
  `finance_review_date` datetime DEFAULT NULL COMMENT '财务审核日期',
  `finance_review_moment` varchar(100) DEFAULT NULL COMMENT '财务审核意见',
  `submit_reviewer_id` int(11) DEFAULT NULL COMMENT '提交审核人员ID',
  `submit_review_date` datetime DEFAULT NULL COMMENT '提交审核日期',
  `submit_review_moment` varchar(100) DEFAULT NULL COMMENT '提交审核意见',
  `review_status` int(11) DEFAULT NULL COMMENT '审核状体 未审核：-1；未通过：0；已通过：1',
  `date_finish` datetime DEFAULT NULL COMMENT '完成时间',
  `creator_id` int(11) DEFAULT NULL COMMENT '创建者',
  `accountant_id` int(11) DEFAULT NULL COMMENT '会计',
  `salesman_id` int(11) DEFAULT NULL COMMENT '业务员',
  `manager_id` int(11) DEFAULT NULL COMMENT '经理',
  `description` varchar(100) DEFAULT NULL,
  `turnover_currency` varchar(10) DEFAULT NULL COMMENT '营业额币别',
  `assistant_id` int(11) DEFAULT NULL COMMENT '助理',
  `date_created` datetime DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `customer_id` (`customer_id`),
  KEY `master_id` (`master_id`),
  KEY `creator_id` (`creator_id`),
  KEY `salesman_id` (`salesman_id`),
  KEY `accountant_id` (`accountant_id`),
  KEY `manager_id` (`manager_id`),
  KEY `finance_reviewer_id` (`finance_reviewer_id`),
  KEY `submit_reviewer_id` (`submit_reviewer_id`),
  CONSTRAINT `audit_sub_ibfk_customer` FOREIGN KEY (`customer_id`) REFERENCES `customer` (`id`),
  CONSTRAINT `audit_sub_ibfk_creator` FOREIGN KEY (`creator_id`) REFERENCES `member` (`id`),
  CONSTRAINT `audit_sub_ibfk_salesman` FOREIGN KEY (`salesman_id`) REFERENCES `member` (`id`),
  CONSTRAINT `audit_sub_ibfk_accountant` FOREIGN KEY (`accountant_id`) REFERENCES `member` (`id`),
  CONSTRAINT `audit_sub_ibfk_manager` FOREIGN KEY (`manager_id`) REFERENCES `member` (`id`),
  CONSTRAINT `audit_sub_ibfk_finance_reviewer` FOREIGN KEY (`finance_reviewer_id`) REFERENCES `member` (`id`),
  CONSTRAINT `audit_sub_ibfk_submit_reviewer` FOREIGN KEY (`submit_reviewer_id`) REFERENCES `member` (`id`),
  CONSTRAINT `audit_sub_ibfk_master` FOREIGN KEY (`master_id`) REFERENCES `audit` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
INSERT INTO `sequence` VALUES ('sub_audit', '1');