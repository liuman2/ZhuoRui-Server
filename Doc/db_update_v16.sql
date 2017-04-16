DROP TABLE IF EXISTS `accounting`;
CREATE TABLE `accounting` (
  `id` int(11) NOT NULL,
  `customer_id` int(11) NULL,
  `code` varchar(20) DEFAULT NULL COMMENT '单号',
  `source_type` int(11) NULL COMMENT '0-国内注册，1-转秘书',
  `source_id` int(11) NULL,
  `source_code` varchar(20) DEFAULT NULL COMMENT '来源单号',

  `name` varchar(100) DEFAULT NULL COMMENT '公司名称',
  `legal` varchar(20) DEFAULT NULL COMMENT '法人',
  `address` varchar(300) DEFAULT NULL COMMENT '公司地址',
  `bank_id` int(11) NULL COMMENT '开户行ID',
  `date_transaction` datetime DEFAULT NULL COMMENT '成交日期',
  `amount_transaction` float(255,2) DEFAULT NULL COMMENT '成交金额',
  `currency` varchar(10) DEFAULT NULL COMMENT '币别',
  `rate` float(255,2) DEFAULT NULL COMMENT '汇率',

  `status` tinyint(3) NULL COMMENT '订单状态 状态:0-未提交, 1-已提交, 2-财务已审核, 3-提交人已审核, 4-完成',
  `finance_reviewer_id` int(11) DEFAULT NULL COMMENT '财务审核人员ID',
  `finance_review_date` datetime DEFAULT NULL COMMENT '财务审核日期',
  `finance_review_moment` varchar(100) DEFAULT NULL COMMENT '财务审核意见',

  `submit_reviewer_id` int(11) DEFAULT NULL COMMENT '提交审核人员ID',
  `submit_review_date` datetime DEFAULT NULL COMMENT '提交审核日期',
  `submit_review_moment` varchar(100) DEFAULT NULL COMMENT '提交审核意见',

  `review_status` int(11) DEFAULT NULL COMMENT '审核状体 未审核：-1；未通过：0；已通过：1',

  `organization_id` int(11) DEFAULT NULL COMMENT '业务员部门',
  `creator_id` int(11) DEFAULT NULL COMMENT '创建者',
  `salesman_id` int(11) DEFAULT NULL COMMENT '业务员',
  `accountant_id` int(11) DEFAULT NULL COMMENT '会计',
  `manager_id` int(11) DEFAULT NULL COMMENT '经理',
  `assistant_id` int(11) DEFAULT NULL COMMENT '助理',
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,

  PRIMARY KEY (`id`),
  KEY `customer_id` (`customer_id`),
  KEY `creator_id` (`creator_id`),
  KEY `salesman_id` (`salesman_id`),
  KEY `accountant_id` (`accountant_id`),
  KEY `manager_id` (`manager_id`),
  KEY `assistant_id` (`assistant_id`),
  KEY `finance_reviewer_id` (`finance_reviewer_id`),
  KEY `submit_reviewer_id` (`submit_reviewer_id`),
  KEY `bank_id` (`bank_id`),

  CONSTRAINT `accounting_ibfk_customer` FOREIGN KEY (`customer_id`) REFERENCES `customer` (`id`),
  CONSTRAINT `accounting_ibfk_creator` FOREIGN KEY (`creator_id`) REFERENCES `member` (`id`),
  CONSTRAINT `accounting_ibfk_salesman` FOREIGN KEY (`salesman_id`) REFERENCES `member` (`id`),
  CONSTRAINT `accounting_ibfk_accountant` FOREIGN KEY (`accountant_id`) REFERENCES `member` (`id`),
  CONSTRAINT `accounting_ibfk_manager` FOREIGN KEY (`manager_id`) REFERENCES `member` (`id`),
  CONSTRAINT `accounting_ibfk_oassistant` FOREIGN KEY (`assistant_id`) REFERENCES `member` (`id`),
  CONSTRAINT `accounting_ibfk_finance_reviewer` FOREIGN KEY (`finance_reviewer_id`) REFERENCES `member` (`id`),
  CONSTRAINT `accounting_ibfk_submit_reviewer` FOREIGN KEY (`submit_reviewer_id`) REFERENCES `member` (`id`),
  CONSTRAINT `accounting_ibfk_bank` FOREIGN KEY (`bank_id`) REFERENCES `bank_account` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
INSERT INTO `sequence` VALUES ('accounting', '1');

DROP TABLE IF EXISTS `accounting_item`;
CREATE TABLE `accounting_item` (
  `id` int(11) NOT NULL,
  `master_id` int(11) NOT NULL,
  `date_start` datetime DEFAULT NULL,
  `date_end` datetime DEFAULT NULL,

  `status` int(11) DEFAULT NULL COMMENT '状态 未完成：0；已完成：1',
  `memo` varchar(500) DEFAULT NULL COMMENT '备注',
  `finisher` varchar(30) DEFAULT NULL COMMENT '负责人',

  `date_created` datetime DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
INSERT INTO `sequence` VALUES ('accounting_item', '1');


