
SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for sequence
-- ----------------------------
DROP TABLE IF EXISTS `sequence`;
CREATE TABLE `sequence` (
  `name` varchar(50) NOT NULL,
  `id` bigint(20) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
-- ----------------------------
-- Records of sequence
-- ----------------------------
INSERT INTO `sequence` VALUES ('organization', '13');
INSERT INTO `sequence` VALUES ('position', '11');
INSERT INTO `sequence` VALUES ('role', '6');
INSERT INTO `sequence` VALUES ('member', '2');
INSERT INTO `sequence` VALUES ('area', '4');
INSERT INTO `sequence` VALUES ('dictionary_group', '13');
INSERT INTO `sequence` VALUES ('dictionary', '15');
INSERT INTO `sequence` VALUES ('customer', '1');
INSERT INTO `sequence` VALUES ('bank_account', '1');
INSERT INTO `sequence` VALUES ('reg_abroad', '1');
INSERT INTO `sequence` VALUES ('customer_timeline', '1');
INSERT INTO `sequence` VALUES ('income', '1');
INSERT INTO `sequence` VALUES ('timeline', '1');
INSERT INTO `sequence` VALUES ('reg_history', '1');
INSERT INTO `sequence` VALUES ('reg_internal_history', '1');
INSERT INTO `sequence` VALUES ('reg_internal', '1');
INSERT INTO `sequence` VALUES ('audit', '1');
INSERT INTO `sequence` VALUES ('trademark', '1');
INSERT INTO `sequence` VALUES ('patent', '1');
INSERT INTO `sequence` VALUES ('settings', '1');
INSERT INTO `sequence` VALUES ('annual_exam', '1');
INSERT INTO `sequence` VALUES ('role_operation', '1');


-- ----------------------------
-- Table structure for organization
-- ----------------------------
DROP TABLE IF EXISTS `organization`;
CREATE TABLE `organization` (
  `id` int(11) NOT NULL,
  `parent_id` int(11) NULL,
  `name` varchar(20) NOT NULL,
  `description` varchar(100) NULL,
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='组织架构';
-- ----------------------------
-- Records of organization
-- ----------------------------
INSERT INTO `organization` (id, parent_id, name) VALUES ('1', '0', '卓瑞企业');
INSERT INTO `organization` (id, parent_id, name) VALUES ('2', '1', '厦门分公司');
INSERT INTO `organization` (id, parent_id, name) VALUES ('3', '2', '综合管理中心');
INSERT INTO `organization` (id, parent_id, name) VALUES ('4', '2', '营销中心');
INSERT INTO `organization` (id, parent_id, name) VALUES ('5', '3', '行政部');
INSERT INTO `organization` (id, parent_id, name) VALUES ('6', '3', '财务部');
INSERT INTO `organization` (id, parent_id, name) VALUES ('7', '4', '注册部');
INSERT INTO `organization` (id, parent_id, name) VALUES ('8', '4', '商标部');
INSERT INTO `organization` (id, parent_id, name) VALUES ('9', '4', '审计部');
INSERT INTO `organization` (id, parent_id, name) VALUES ('10', '4', '年检部');
INSERT INTO `organization` (id, parent_id, name) VALUES ('11', '4', '泉州营销部');
INSERT INTO `organization` (id, parent_id, name) VALUES ('12', '4', '青岛营销部');

-- ----------------------------
-- Table structure for position
-- ----------------------------
DROP TABLE IF EXISTS `position`;
CREATE TABLE `position` (
  `id` int(11) NOT NULL,
  `name` varchar(20) NOT NULL,
  `description` varchar(100) NULL,
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='职位管理';
-- ----------------------------
-- Records of position
-- ----------------------------
INSERT INTO `position` (id, name) VALUES ('1', '经理');
INSERT INTO `position` (id, name) VALUES ('2', '业务组长');
INSERT INTO `position` (id, name) VALUES ('3', '业务副组长');
INSERT INTO `position` (id, name) VALUES ('4', '组长助理');
INSERT INTO `position` (id, name) VALUES ('5', '高级顾问');
INSERT INTO `position` (id, name) VALUES ('6', '商务顾问');
INSERT INTO `position` (id, name) VALUES ('7', '商务助理');
INSERT INTO `position` (id, name) VALUES ('8', '销售文员');
INSERT INTO `position` (id, name) VALUES ('9', '会计');
INSERT INTO `position` (id, name) VALUES ('10', '出纳');

-- ----------------------------
-- Table structure for role
-- ----------------------------
DROP TABLE IF EXISTS `role`;
CREATE TABLE `role` (
  `id` int(11) NOT NULL,
  `name` varchar(20) NOT NULL,
  `code` varchar(20) NOT NULL,
  `is_system` tinyint(4) DEFAULT NULL,
  `description` varchar(100) NULL,
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='角色管理';
-- ----------------------------
-- Records of role
-- ----------------------------
INSERT INTO `role` (id, name, code, is_system, description) VALUES ('1', '系统管理员',  'admin',    '1',  '拥有最高权限');
INSERT INTO `role` (id, name, code, is_system, description) VALUES ('2', '总经理',      'manager',  '1',  '可查看公司所有业务资料');
INSERT INTO `role` (id, name, code, is_system, description) VALUES ('3', '部门经理',    'leader',   '1',  '可查看所属部门员工的客户资料和订单资料');
INSERT INTO `role` (id, name, code, is_system, description) VALUES ('4', '业务员',      'employee', '1',  '只能查看自己的客户资料和订单资料');
INSERT INTO `role` (id, name, code, is_system, description) VALUES ('5', '财务',        'finance',  '1',  '财务审核权限');
INSERT INTO `role` (id, name, code, is_system, description) VALUES ('6', '提交',        'submit',   '1',  '订单提交人');

-- ----------------------------
-- Table structure for member
-- ----------------------------
DROP TABLE IF EXISTS `member`;
CREATE TABLE `member` (
  `id` int(11) NOT NULL,
  `organization_id` int(11) DEFAULT NULL,
  `area_id` int(11) DEFAULT NULL,
  `position_id` int(11) DEFAULT NULL,
  `username` varchar(20) NOT NULL,
  `name` varchar(20) NOT NULL,
  `english_name` varchar(20) DEFAULT NULL,
  `password` varchar(100) DEFAULT NULL,
  `mobile` varchar(20) DEFAULT NULL,
  `hiredate` datetime DEFAULT NULL,
  `birthday` datetime DEFAULT NULL,
  `status` int(11) DEFAULT NULL COMMENT '在职：1；离职：0；停薪留职：2',
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `organization_id` (`organization_id`),
  KEY `position_id` (`position_id`),
  KEY `area_id` (`area_id`),
  CONSTRAINT `member_ibfk_1` FOREIGN KEY (`organization_id`) REFERENCES `organization` (`id`),
  CONSTRAINT `member_ibfk_2` FOREIGN KEY (`position_id`) REFERENCES `position` (`id`),
  CONSTRAINT `member_ibfk_3` FOREIGN KEY (`area_id`) REFERENCES `area` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='员工管理表';
INSERT INTO `member` VALUES ('1', '1', '1', null, 'admin', '系统管理员', 'admin', 'M835cqqAkhHw/dbOtLYjyKjnKfo=', null, null, null, null, '2016-08-03 14:08:18', null);

-- ----------------------------
-- Table structure for area
-- ----------------------------
DROP TABLE IF EXISTS `area`;
CREATE TABLE `area` (
  `id` int(11) NOT NULL,
  `name` varchar(20) NOT NULL,
  `description` varchar(100) NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='区域管理';
-- ----------------------------
-- Records of role
-- ----------------------------
INSERT INTO `area` (id, name) VALUES ('1', '厦门');
INSERT INTO `area` (id, name) VALUES ('2', '泉州');
INSERT INTO `area` (id, name) VALUES ('3', '青岛');

-- ----------------------------
-- Table structure for area
-- ----------------------------
DROP TABLE IF EXISTS `dictionary_group`;
CREATE TABLE `dictionary_group` (
  `id` int(11) NOT NULL,
  `group` varchar(20) NULL,
  `name` varchar(20) NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='数据类别';
-- ----------------------------
-- Records of dictionary_type
-- ----------------------------
INSERT INTO `dictionary_group` VALUES ('1', '行业类别', '行业类别');
INSERT INTO `dictionary_group` VALUES ('2', '业务性质', '业务性质');
INSERT INTO `dictionary_group` VALUES ('3', '业务范围', '业务范围');
INSERT INTO `dictionary_group` VALUES ('4', '客户来源',  '客户来源');
INSERT INTO `dictionary_group` VALUES ('5', '贸易方式', '贸易方式');
-- INSERT INTO `dictionary_group` VALUES ('6', '商标类别', '商标类别');
INSERT INTO `dictionary_group` VALUES ('7', '商标注册方式', '商标注册方式');
INSERT INTO `dictionary_group` VALUES ('8', '专利注册方式', '专利注册方式');
INSERT INTO `dictionary_group` VALUES ('9', '专利类型', '专利类型');
INSERT INTO `dictionary_group` VALUES ('10', '专利用途', '专利用途');
INSERT INTO `dictionary_group` VALUES ('11', '注册地区', '注册地区');
INSERT INTO `dictionary_group` VALUES ('12', '纳税人资格', '纳税人资格');
INSERT INTO `dictionary_group` VALUES ('13', '币别', '币别');
INSERT INTO `dictionary_group` VALUES ('14', '付款方式', '付款方式');
-- ----------------------------
-- Table structure for dictionary
-- ----------------------------
DROP TABLE IF EXISTS `dictionary`;
CREATE TABLE `dictionary` (
  `id` int(11) NOT NULL,
  `group` varchar(20) DEFAULT NULL,
  `name` varchar(50) DEFAULT NULL,
  `is_system` tinyint(3) NULL,
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

INSERT INTO `dictionary` VALUES ('1', '客户来源', '网上搜索', 1, null, null);
INSERT INTO `dictionary` VALUES ('2', '客户来源', '现场开发', 1, null, null);
INSERT INTO `dictionary` VALUES ('3', '客户来源', '电话开发', 1, null, null);
INSERT INTO `dictionary` VALUES ('4', '客户来源', '视频开发', 1, null, null);
INSERT INTO `dictionary` VALUES ('5', '客户来源', '客户介绍', 1, null, null);
INSERT INTO `dictionary` VALUES ('6', '纳税人资格', '一般纳税人', 1, null, null);
INSERT INTO `dictionary` VALUES ('7', '纳税人资格', '小规模纳税人', 1, null, null);
INSERT INTO `dictionary` VALUES ('8', '币别', '人民币', 1, null, null);
INSERT INTO `dictionary` VALUES ('9', '币别', '港币', 1, null, null);
INSERT INTO `dictionary` VALUES ('10', '币别', '美元', 1, null, null);

INSERT INTO `dictionary` VALUES ('11', '付款方式', '现金', 1, null, null);
INSERT INTO `dictionary` VALUES ('12', '付款方式', '转账', 1, null, null);
INSERT INTO `dictionary` VALUES ('13', '付款方式', '其他方式', 1, null, null);
INSERT INTO `dictionary` VALUES ('14', '付款方式', '先提交,未付款', 1, null, null);

-- ----------------------------
-- Table structure for customer
-- ----------------------------
DROP TABLE IF EXISTS `customer`;
CREATE TABLE `customer` (
  `id` int(11) NOT NULL,
  `code` varchar(20) DEFAULT NULL COMMENT '客户编码',
  `name` varchar(100) DEFAULT NULL COMMENT '客户名称',
  `industry` varchar(50) DEFAULT NULL COMMENT '所属行业',
  `business_nature` varchar(50) DEFAULT NULL COMMENT '业务性质',
  `province` varchar(20) DEFAULT NULL,
  `city` varchar(20) DEFAULT NULL,
  `county` varchar(20) DEFAULT NULL,
  `address` varchar(200) DEFAULT NULL,
  `contact` varchar(30) DEFAULT NULL COMMENT '联系人',
  `mobile` varchar(20) DEFAULT NULL,
  `tel` varchar(20) DEFAULT NULL,
  `fax` varchar(20) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `QQ` varchar(50) DEFAULT NULL,
  `wechat` varchar(50) DEFAULT NULL,
  `source` varchar(50) DEFAULT NULL COMMENT '客户来源',
  `creator_id` int(11) DEFAULT NULL COMMENT '创建者',
  `salesman_id` int(11) DEFAULT NULL COMMENT '业务员',

  `organization_id` int(11) DEFAULT NULL COMMENT '业务员部门',
  `source_id` int(11) DEFAULT NULL COMMENT '介绍人',
  `status` tinyint(3) DEFAULT NULL COMMENT '状态，0-预备，1-正式',
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  `description` varchar(100) NULL,
  PRIMARY KEY (`id`),
  KEY `creator_id` (`creator_id`),
  KEY `salesman_id` (`salesman_id`),
  CONSTRAINT `member_ibfk_creator` FOREIGN KEY (`creator_id`) REFERENCES `member` (`id`),
  CONSTRAINT `member_ibfk_salesman` FOREIGN KEY (`salesman_id`) REFERENCES `member` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `customer_timeline`;
CREATE TABLE `customer_timeline` (
  `id` int(11) NOT NULL,
  `customer_id` int(11) NULL,
  `title` varchar(100) DEFAULT NULL,
  `content` varchar(500) DEFAULT NULL,
  `is_system` tinyint(3) DEFAULT NULL,
  `date_business` datetime NULL NULL,
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `customer_id` (`customer_id`),
  CONSTRAINT `timeline_ibfk_customer` FOREIGN KEY (`customer_id`) REFERENCES `customer` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `bank_account`;
CREATE TABLE `bank_account` (
  `id` int(11) NOT NULL,
  `customer_id` int(11) NULL,
  `bank` varchar(100) DEFAULT NULL COMMENT '开户行',
  `holder` varchar(30) DEFAULT NULL COMMENT '开户人',
  `account` varchar(100) DEFAULT NULL COMMENT '银行账号',
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `customer_id` (`customer_id`),
  CONSTRAINT `bank_account_customer` FOREIGN KEY (`customer_id`) REFERENCES `customer` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `reg_abroad`;
CREATE TABLE `reg_abroad` (
  `id` int(11) NOT NULL,
  `customer_id` int(11) NULL,
  `code` varchar(20) DEFAULT NULL COMMENT '单号',
  `name_cn` varchar(100) DEFAULT NULL COMMENT '公司中文名称',
  `name_en` varchar(100) DEFAULT NULL COMMENT '公司英文名称',
  `date_setup` datetime DEFAULT NULL COMMENT '公司成立日期',
  `reg_no` varchar(100) DEFAULT NULL COMMENT '公司注册编号',
  `region` varchar(50) DEFAULT NULL COMMENT '公司注册地区',
  `address` varchar(300) DEFAULT NULL COMMENT '公司注册地址',
  `director` varchar(20) DEFAULT NULL COMMENT '公司董事',
  `is_open_bank` tinyint(3) NULL COMMENT '是否开户',
  `bank_id` int(11) NULL COMMENT '开户行ID',
  `date_transaction` datetime DEFAULT NULL COMMENT '成交日期',
  `amount_transaction` float(255,2) DEFAULT NULL COMMENT '成交金额',
  `currency` varchar(10) DEFAULT NULL COMMENT '币别',
  `rate` float(255,2) DEFAULT NULL COMMENT '汇率',

  `invoice_name` varchar(200) DEFAULT NULL COMMENT '开票信息名称',
  `invoice_tax` varchar(200) DEFAULT NULL COMMENT '开票信息纳税人识别号',
  `invoice_address` varchar(200) DEFAULT NULL COMMENT '开票信息地址',
  `invoice_tel` varchar(20) DEFAULT NULL COMMENT '开票信息电话',
  `invoice_bank` varchar(100) DEFAULT NULL COMMENT '开票信息开户行',
  `invoice_account` varchar(100) DEFAULT NULL COMMENT '开票信息开户行账号',

  `status` tinyint(3) NULL COMMENT '订单状态 状态:0-未提交, 1-已提交, 2-财务已审核, 3-提交人已审核, 4-完成',
  `finance_reviewer_id` int(11) DEFAULT NULL COMMENT '财务审核人员ID',
  `finance_review_date` datetime DEFAULT NULL COMMENT '财务审核日期',
  `finance_review_moment` varchar(100) DEFAULT NULL COMMENT '财务审核意见',

  `submit_reviewer_id` int(11) DEFAULT NULL COMMENT '提交审核人员ID',
  `submit_review_date` datetime DEFAULT NULL COMMENT '提交审核日期',
  `submit_review_moment` varchar(100) DEFAULT NULL COMMENT '提交审核意见',

  `review_status` int(11) DEFAULT NULL COMMENT '审核状体 未审核：-1；未通过：0；已通过：1',
  `date_finish` datetime DEFAULT NULL COMMENT '完成时间',

  `progress` varchar(50) DEFAULT NULL COMMENT '注册进度',

  `creator_id` int(11) DEFAULT NULL COMMENT '创建者',
  `salesman_id` int(11) DEFAULT NULL COMMENT '业务员',
  `waiter_id` int(11) DEFAULT NULL COMMENT '年检客服',
  `manager_id` int(11) DEFAULT NULL COMMENT '经理',
  `outworker_id` int(11) DEFAULT NULL COMMENT '外勤',
  `organization_id` int(11) DEFAULT NULL COMMENT '业务员部门',
  `description` varchar(100) NULL,

  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  `annual_year` int(11) DEFAULT NULL COMMENT '上次年检年份',

  PRIMARY KEY (`id`),
  KEY `customer_id` (`customer_id`),
  KEY `creator_id` (`creator_id`),
  KEY `salesman_id` (`salesman_id`),
  KEY `waiter_id` (`waiter_id`),
  KEY `manager_id` (`manager_id`),
  KEY `outworker_id` (`outworker_id`),
  KEY `finance_reviewer_id` (`finance_reviewer_id`),
  KEY `submit_reviewer_id` (`submit_reviewer_id`),
  KEY `bank_id` (`bank_id`),

  CONSTRAINT `abroad_ibfk_customer` FOREIGN KEY (`customer_id`) REFERENCES `customer` (`id`),
  CONSTRAINT `abroad_ibfk_creator` FOREIGN KEY (`creator_id`) REFERENCES `member` (`id`),
  CONSTRAINT `abroad_ibfk_salesman` FOREIGN KEY (`salesman_id`) REFERENCES `member` (`id`),
  CONSTRAINT `abroad_ibfk_waiter` FOREIGN KEY (`waiter_id`) REFERENCES `member` (`id`),
  CONSTRAINT `abroad_ibfk_manager` FOREIGN KEY (`manager_id`) REFERENCES `member` (`id`),
  CONSTRAINT `abroad_ibfk_outworker` FOREIGN KEY (`outworker_id`) REFERENCES `member` (`id`),
  CONSTRAINT `abroad_ibfk_finance_reviewer` FOREIGN KEY (`finance_reviewer_id`) REFERENCES `member` (`id`),
  CONSTRAINT `abroad_ibfk_submit_reviewer` FOREIGN KEY (`submit_reviewer_id`) REFERENCES `member` (`id`),
  CONSTRAINT `abroad_ibfk_bank` FOREIGN KEY (`bank_id`) REFERENCES `bank_account` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `reg_history`;
CREATE TABLE `reg_history` (
  `id` int(11) NOT NULL,
  `reg_id` int(11) NULL,
  `name_cn` varchar(100) DEFAULT NULL COMMENT '公司中文名称',
  `name_en` varchar(100) DEFAULT NULL COMMENT '公司英文名称',
  `date_setup` datetime DEFAULT NULL COMMENT '公司成立日期',
  `reg_no` varchar(100) DEFAULT NULL COMMENT '公司注册编号',
  `region` varchar(50) DEFAULT NULL COMMENT '公司注册地区',
  `address` varchar(300) DEFAULT NULL COMMENT '公司注册地址',
  `director` varchar(20) DEFAULT NULL COMMENT '公司董事',
  `others` varchar(100) DEFAULT NULL COMMENT '其他变更',
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `reg_internal`;
CREATE TABLE `reg_internal` (
  `id` int(11) NOT NULL,
  `customer_id` int(11) NULL,
  `code` varchar(20) DEFAULT NULL COMMENT '单号',
  `name_cn` varchar(100) DEFAULT NULL COMMENT '公司中文名称',

  `date_setup` datetime DEFAULT NULL COMMENT '公司成立日期',
  `reg_no` varchar(100) DEFAULT NULL COMMENT '公司统一信用编号',
  `address` varchar(300) DEFAULT NULL COMMENT '公司注册地址',

  `legal` varchar(20) DEFAULT NULL COMMENT '公司法人',
  `director` varchar(20) DEFAULT NULL COMMENT '公司监事',
  `bank_id` int(11) NULL COMMENT '开户行ID',
  `taxpayer` varchar(10) DEFAULT NULL COMMENT '纳税人资格',

  `is_customs` tinyint(3) NULL COMMENT '是否海关备案',
  `customs_name` varchar(300) DEFAULT NULL COMMENT '海关备案英文名称',
  `customs_address` varchar(300) DEFAULT NULL COMMENT '海关备案英文地址',

  `date_transaction` datetime DEFAULT NULL COMMENT '成交日期',
  `amount_transaction` float(255,2) DEFAULT NULL COMMENT '成交金额',
  `currency` varchar(10) DEFAULT NULL COMMENT '币别',
  `rate` float(255,2) DEFAULT NULL COMMENT '汇率',

  `is_bookkeeping` tinyint(3) NULL COMMENT '是否在我司代理记账',
  `amount_bookkeeping` float(255,2) DEFAULT NULL COMMENT '代理记账费用',

  `invoice_name` varchar(200) DEFAULT NULL COMMENT '开票信息名称',
  `invoice_tax` varchar(200) DEFAULT NULL COMMENT '开票信息纳税人识别号',
  `invoice_address` varchar(200) DEFAULT NULL COMMENT '开票信息地址',
  `invoice_tel` varchar(20) DEFAULT NULL COMMENT '开票信息电话',
  `invoice_bank` varchar(100) DEFAULT NULL COMMENT '开票信息开户行',
  `invoice_account` varchar(100) DEFAULT NULL COMMENT '开票信息开户行账号',

  `status` tinyint(3) NULL COMMENT '订单状态 状态:0-未提交, 1-已提交, 2-财务已审核, 3-提交人已审核, 4-完成',
  `finance_reviewer_id` int(11) DEFAULT NULL COMMENT '财务审核人员ID',
  `finance_review_date` datetime DEFAULT NULL COMMENT '财务审核日期',
  `finance_review_moment` varchar(100) DEFAULT NULL COMMENT '财务审核意见',

  `submit_reviewer_id` int(11) DEFAULT NULL COMMENT '提交审核人员ID',
  `submit_review_date` datetime DEFAULT NULL COMMENT '提交审核日期',
  `submit_review_moment` varchar(100) DEFAULT NULL COMMENT '提交审核意见',

  `review_status` int(11) DEFAULT NULL COMMENT '审核状体 未审核：-1；未通过：0；已通过：1',
  `date_finish` datetime DEFAULT NULL COMMENT '完成时间',

  `progress` varchar(50) DEFAULT NULL COMMENT '注册进度',

  `creator_id` int(11) DEFAULT NULL COMMENT '创建者',
  `salesman_id` int(11) DEFAULT NULL COMMENT '业务员',
  `waiter_id` int(11) DEFAULT NULL COMMENT '年检客服',
  `manager_id` int(11) DEFAULT NULL COMMENT '经理',
  `outworker_id` int(11) DEFAULT NULL COMMENT '外勤',
  `organization_id` int(11) DEFAULT NULL COMMENT '业务员部门',
  `description` varchar(100) NULL,

  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  `annual_year` int(11) DEFAULT NULL COMMENT '上次年检年份',
  PRIMARY KEY (`id`),
  KEY `customer_id` (`customer_id`),
  KEY `creator_id` (`creator_id`),
  KEY `salesman_id` (`salesman_id`),
  KEY `waiter_id` (`waiter_id`),
  KEY `manager_id` (`manager_id`),
  KEY `outworker_id` (`outworker_id`),
  KEY `finance_reviewer_id` (`finance_reviewer_id`),
  KEY `submit_reviewer_id` (`submit_reviewer_id`),
  KEY `bank_id` (`bank_id`),

  CONSTRAINT `reg_internal_ibfk_customer` FOREIGN KEY (`customer_id`) REFERENCES `customer` (`id`),
  CONSTRAINT `reg_internal_ibfk_creator` FOREIGN KEY (`creator_id`) REFERENCES `member` (`id`),
  CONSTRAINT `reg_internal_ibfk_salesman` FOREIGN KEY (`salesman_id`) REFERENCES `member` (`id`),
  CONSTRAINT `reg_internal_ibfk_waiter` FOREIGN KEY (`waiter_id`) REFERENCES `member` (`id`),
  CONSTRAINT `reg_internal_ibfk_manager` FOREIGN KEY (`manager_id`) REFERENCES `member` (`id`),
  CONSTRAINT `reg_internal_ibfk_outworker` FOREIGN KEY (`outworker_id`) REFERENCES `member` (`id`),
  CONSTRAINT `reg_internal_ibfk_finance_reviewer` FOREIGN KEY (`finance_reviewer_id`) REFERENCES `member` (`id`),
  CONSTRAINT `reg_internal_ibfk_submit_reviewer` FOREIGN KEY (`submit_reviewer_id`) REFERENCES `member` (`id`),
  CONSTRAINT `reg_internal_ibfk_bank` FOREIGN KEY (`bank_id`) REFERENCES `bank_account` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `reg_internal_history`;
CREATE TABLE `reg_internal_history` (
  `id` int(11) NOT NULL,
  `reg_id` int(11) NULL,
  `name_cn` varchar(100) DEFAULT NULL COMMENT '公司中文名称',
  `date_setup` datetime DEFAULT NULL COMMENT '公司成立日期',
  `reg_no` varchar(100) DEFAULT NULL COMMENT '公司注册编号',
  `address` varchar(300) DEFAULT NULL COMMENT '公司注册地址',
  `legal` varchar(20) DEFAULT NULL COMMENT '公司法人',
  `director` varchar(20) DEFAULT NULL COMMENT '公司董事',
  `others` varchar(100) DEFAULT NULL COMMENT '其他变更',
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `income`;
CREATE TABLE `income` (
  `id` int(11) NOT NULL,
  `customer_id` int(11) NULL,
  `source_id` int(11) NULL,
  `source_name` varchar(20) DEFAULT NULL,
  `payer` varchar(100) DEFAULT NULL COMMENT '付款人',
  `pay_way` varchar(100) DEFAULT NULL COMMENT '收款方式',
  `account` varchar(100) DEFAULT NULL COMMENT '收款账号',
  `bank` varchar(100) DEFAULT NULL COMMENT '收款银行',
  `amount` float(255,2) DEFAULT NULL COMMENT '收款金额',

  `rate` float(255,2) DEFAULT NULL COMMENT '汇率',
  `currency` varchar(10) DEFAULT NULL COMMENT '币别',

  `date_pay` datetime DEFAULT NULL COMMENT '收款时间',
  `attachment_url` varchar(100) DEFAULT NULL COMMENT '附件地址',
  `description` varchar(100) NULL,
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `customer_id` (`customer_id`),
  CONSTRAINT `income_ibfk_customer` FOREIGN KEY (`customer_id`) REFERENCES `customer` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `timeline`;
CREATE TABLE `timeline` (
  `id` int(11) NOT NULL,
  `source_id` int(11) NULL,
  `source_name` varchar(20) DEFAULT NULL,
  `title` varchar(100) DEFAULT NULL,
  `content` varchar(500) DEFAULT NULL,
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `audit`;
CREATE TABLE `audit` (
  `id` int(11) NOT NULL,
  `customer_id` int(11) NULL,
  `code` varchar(20) DEFAULT NULL COMMENT '单号',
  `type` varchar(10) DEFAULT NULL COMMENT '单据类别境内外',
  `name_cn` varchar(100) DEFAULT NULL COMMENT '公司中文名称',
  `name_en` varchar(100) DEFAULT NULL COMMENT '公司英文名称',
  `date_setup` datetime DEFAULT NULL COMMENT '公司成立日期',
  `address` varchar(300) DEFAULT NULL COMMENT '公司注册地址',
  `business_area` varchar(60) DEFAULT NULL COMMENT '业务范围',
  `trade_mode` varchar(60) DEFAULT NULL COMMENT '贸易方式',
  `has_parent` tinyint(3) NULL COMMENT '有无子母公司',
  `account_number` int(11) NULL COMMENT '做账次数',
  `account_period` datetime DEFAULT NULL COMMENT '账期',
  -- `account_period_end` datetime DEFAULT NULL COMMENT '结束账期',
  `date_year_end` datetime DEFAULT NULL COMMENT '年结日',
  `turnover` float(255,2) DEFAULT NULL COMMENT '营业额',
  `amount_bank` float(255,2) DEFAULT NULL COMMENT '银行入账金额',
  `bill_number` int(11) NULL COMMENT '单据量',
  `accounting_standard` varchar(50) DEFAULT NULL COMMENT '会计准则',
  `cost_accounting` float(255,2) DEFAULT NULL COMMENT '做账费用',
  `date_transaction` datetime DEFAULT NULL COMMENT '成交日期',
  `amount_transaction` float(255,2) DEFAULT NULL COMMENT '成交金额',
  `currency` varchar(10) DEFAULT NULL COMMENT '币别',
  `rate` float(255,2) DEFAULT NULL COMMENT '汇率',

  `progress` varchar(50) DEFAULT NULL COMMENT '审计进度',

  `status` tinyint(3) NULL COMMENT '订单状态 状态:0-未提交, 1-已提交, 2-财务已审核, 3-提交人已审核, 4-完成',
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
  `organization_id` int(11) DEFAULT NULL COMMENT '业务员部门',
  `description` varchar(100) NULL,
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  `annual_year` int(11) DEFAULT NULL COMMENT '上次年检年份',
  PRIMARY KEY (`id`),
  KEY `customer_id` (`customer_id`),
  KEY `creator_id` (`creator_id`),
  KEY `salesman_id` (`salesman_id`),
  KEY `accountant_id` (`accountant_id`),
  KEY `manager_id` (`manager_id`),
  KEY `finance_reviewer_id` (`finance_reviewer_id`),
  KEY `submit_reviewer_id` (`submit_reviewer_id`),

  CONSTRAINT `audit_ibfk_customer` FOREIGN KEY (`customer_id`) REFERENCES `customer` (`id`),
  CONSTRAINT `audit_ibfk_creator` FOREIGN KEY (`creator_id`) REFERENCES `member` (`id`),
  CONSTRAINT `audit_ibfk_salesman` FOREIGN KEY (`salesman_id`) REFERENCES `member` (`id`),
  CONSTRAINT `audit_ibfk_accountant` FOREIGN KEY (`accountant_id`) REFERENCES `member` (`id`),
  CONSTRAINT `audit_ibfk_manager` FOREIGN KEY (`manager_id`) REFERENCES `member` (`id`),
  CONSTRAINT `audit_ibfk_finance_reviewer` FOREIGN KEY (`finance_reviewer_id`) REFERENCES `member` (`id`),
  CONSTRAINT `audit_ibfk_submit_reviewer` FOREIGN KEY (`submit_reviewer_id`) REFERENCES `member` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `trademark`;
CREATE TABLE `trademark` (
  `id` int(11) NOT NULL,
  `customer_id` int(11) NULL,
  `code` varchar(20) DEFAULT NULL COMMENT '单号',
  `type` varchar(10) DEFAULT NULL COMMENT '单据类别境内外',
  `name` varchar(100) DEFAULT NULL COMMENT '商标名称',
  `applicant` varchar(50) DEFAULT NULL COMMENT '申请人',
  `address` varchar(300) DEFAULT NULL COMMENT '申请人地址',
  `trademark_type` varchar(300) DEFAULT NULL COMMENT '商标类别',
  `region` varchar(50) DEFAULT NULL COMMENT '商标地区',
  `reg_mode` varchar(50) DEFAULT NULL COMMENT '注册方式',
  `date_transaction` datetime DEFAULT NULL COMMENT '成交日期',
  `amount_transaction` float(255,2) DEFAULT NULL COMMENT '成交金额',
  `rate` float(255,2) DEFAULT NULL COMMENT '汇率',

  `currency` varchar(10) DEFAULT NULL COMMENT '币别',

  `date_receipt` datetime DEFAULT NULL COMMENT '回执时间',
  `date_accept` datetime DEFAULT NULL COMMENT '受理时间',
  `date_trial` datetime DEFAULT NULL COMMENT '初审时间',
  `date_regit` datetime DEFAULT NULL COMMENT '注册时间',
  `date_exten` datetime DEFAULT NULL COMMENT '续展时间',

  `progress` varchar(50) DEFAULT NULL COMMENT '商标进度',
  `status` tinyint(3) NULL COMMENT '订单状态 状态:0-未提交, 1-已提交, 2-财务已审核, 3-提交人已审核, 4-完成',
  `finance_reviewer_id` int(11) DEFAULT NULL COMMENT '财务审核人员ID',
  `finance_review_date` datetime DEFAULT NULL COMMENT '财务审核日期',
  `finance_review_moment` varchar(100) DEFAULT NULL COMMENT '财务审核意见',

  `submit_reviewer_id` int(11) DEFAULT NULL COMMENT '提交审核人员ID',
  `submit_review_date` datetime DEFAULT NULL COMMENT '提交审核日期',
  `submit_review_moment` varchar(100) DEFAULT NULL COMMENT '提交审核意见',
  `review_status` int(11) DEFAULT NULL COMMENT '审核状体 未审核：-1；未通过：0；已通过：1',
  `date_finish` datetime DEFAULT NULL COMMENT '完成时间',

  `creator_id` int(11) DEFAULT NULL COMMENT '创建者',
  `salesman_id` int(11) DEFAULT NULL COMMENT '业务员',
  `waiter_id` int(11) DEFAULT NULL COMMENT '年检客服',
  `manager_id` int(11) DEFAULT NULL COMMENT '经理',
  `organization_id` int(11) DEFAULT NULL COMMENT '业务员部门',
  `description` varchar(100) NULL,
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  `annual_year` int(11) DEFAULT NULL COMMENT '上次年检年份',
  PRIMARY KEY (`id`),
  KEY `customer_id` (`customer_id`),
  KEY `creator_id` (`creator_id`),
  KEY `salesman_id` (`salesman_id`),
  KEY `waiter_id` (`waiter_id`),
  KEY `manager_id` (`manager_id`),
  KEY `finance_reviewer_id` (`finance_reviewer_id`),
  KEY `submit_reviewer_id` (`submit_reviewer_id`),
  CONSTRAINT `trademark_ibfk_customer` FOREIGN KEY (`customer_id`) REFERENCES `customer` (`id`),
  CONSTRAINT `trademark_ibfk_creator` FOREIGN KEY (`creator_id`) REFERENCES `member` (`id`),
  CONSTRAINT `trademark_ibfk_salesman` FOREIGN KEY (`salesman_id`) REFERENCES `member` (`id`),
  CONSTRAINT `trademark_ibfk_waiter` FOREIGN KEY (`waiter_id`) REFERENCES `member` (`id`),
  CONSTRAINT `trademark_ibfk_manager` FOREIGN KEY (`manager_id`) REFERENCES `member` (`id`),
  CONSTRAINT `trademark_ibfk_finance_reviewer` FOREIGN KEY (`finance_reviewer_id`) REFERENCES `member` (`id`),
  CONSTRAINT `trademark_ibfk_submit_reviewer` FOREIGN KEY (`submit_reviewer_id`) REFERENCES `member` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `patent`;
CREATE TABLE `patent` (
  `id` int(11) NOT NULL,
  `customer_id` int(11) NULL,
  `code` varchar(20) DEFAULT NULL COMMENT '单号',
  `type` varchar(10) DEFAULT NULL COMMENT '单据类别境内外',
  `name` varchar(100) DEFAULT NULL COMMENT '专利名称',
  `applicant` varchar(50) DEFAULT NULL COMMENT '申请人',
  `address` varchar(300) DEFAULT NULL COMMENT '申请人地址',
  `card_no` varchar(300) DEFAULT NULL COMMENT '申请人证件号码',
  `designer` varchar(50) DEFAULT NULL COMMENT '专利设计人',
  `patent_type` varchar(20) DEFAULT NULL COMMENT '专利类型',
  `patent_purpose` varchar(20) DEFAULT NULL COMMENT '专利用途',
  `reg_mode` varchar(50) DEFAULT NULL COMMENT '注册方式',
  `date_transaction` datetime DEFAULT NULL COMMENT '成交日期',
  `amount_transaction` float(255,2) DEFAULT NULL COMMENT '成交金额',
  `rate` float(255,2) DEFAULT NULL COMMENT '汇率',

  `currency` varchar(10) DEFAULT NULL COMMENT '币别',

  `date_accept` datetime DEFAULT NULL COMMENT '受理时间',
  `date_empower` datetime DEFAULT NULL COMMENT '授权时间',
  `date_inspection` datetime DEFAULT NULL COMMENT '年检时间',
  `progress` varchar(50) DEFAULT NULL COMMENT '专利进度',

  `status` tinyint(3) NULL COMMENT '订单状态 状态:0-未提交, 1-已提交, 2-财务已审核, 3-提交人已审核, 4-完成',
  `finance_reviewer_id` int(11) DEFAULT NULL COMMENT '财务审核人员ID',
  `finance_review_date` datetime DEFAULT NULL COMMENT '财务审核日期',
  `finance_review_moment` varchar(100) DEFAULT NULL COMMENT '财务审核意见',
  `submit_reviewer_id` int(11) DEFAULT NULL COMMENT '提交审核人员ID',
  `submit_review_date` datetime DEFAULT NULL COMMENT '提交审核日期',
  `submit_review_moment` varchar(100) DEFAULT NULL COMMENT '提交审核意见',
  `review_status` int(11) DEFAULT NULL COMMENT '审核状体 未审核：-1；未通过：0；已通过：1',
  `date_finish` datetime DEFAULT NULL COMMENT '完成时间',
  `creator_id` int(11) DEFAULT NULL COMMENT '创建者',
  `salesman_id` int(11) DEFAULT NULL COMMENT '业务员',
  `waiter_id` int(11) DEFAULT NULL COMMENT '年检客服',
  `manager_id` int(11) DEFAULT NULL COMMENT '经理',
  `organization_id` int(11) DEFAULT NULL COMMENT '业务员部门',
  `description` varchar(100) NULL,
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  `annual_year` int(11) DEFAULT NULL COMMENT '上次年检年份',
  PRIMARY KEY (`id`),
  KEY `customer_id` (`customer_id`),
  KEY `creator_id` (`creator_id`),
  KEY `salesman_id` (`salesman_id`),
  KEY `waiter_id` (`waiter_id`),
  KEY `manager_id` (`manager_id`),
  KEY `finance_reviewer_id` (`finance_reviewer_id`),
  KEY `submit_reviewer_id` (`submit_reviewer_id`),
  CONSTRAINT `patent_ibfk_customer` FOREIGN KEY (`customer_id`) REFERENCES `customer` (`id`),
  CONSTRAINT `patent_ibfk_creator` FOREIGN KEY (`creator_id`) REFERENCES `member` (`id`),
  CONSTRAINT `patent_ibfk_salesman` FOREIGN KEY (`salesman_id`) REFERENCES `member` (`id`),
  CONSTRAINT `patent_ibfk_waiter` FOREIGN KEY (`waiter_id`) REFERENCES `member` (`id`),
  CONSTRAINT `patent_ibfk_manager` FOREIGN KEY (`manager_id`) REFERENCES `member` (`id`),
  CONSTRAINT `patent_ibfk_finance_reviewer` FOREIGN KEY (`finance_reviewer_id`) REFERENCES `member` (`id`),
  CONSTRAINT `patent_ibfk_submit_reviewer` FOREIGN KEY (`submit_reviewer_id`) REFERENCES `member` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `annual_exam`;
CREATE TABLE `annual_exam` (
  `id` int(11) NOT NULL,
  `customer_id` int(11) NULL,
  `code` varchar(20) DEFAULT NULL COMMENT '单号',
  `type` varchar(10) DEFAULT NULL COMMENT '年检类别',
  `order_id` int(11) NULL COMMENT '年检来源单据',
  `name_cn` varchar(100) DEFAULT NULL COMMENT '公司中文名称',
  `name_en` varchar(100) DEFAULT NULL COMMENT '公司英文名称',
  `order_code` varchar(20) DEFAULT NULL COMMENT '原始单号',
  `amount_transaction` float(255,2) DEFAULT NULL COMMENT '成交金额',
  `date_transaction` datetime DEFAULT NULL COMMENT '成交日期',
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
  `date_finish` datetime DEFAULT NULL COMMENT '完成时间',
  `progress` varchar(50) DEFAULT NULL COMMENT '注册进度',
  `creator_id` int(11) DEFAULT NULL COMMENT '创建者',
  `salesman_id` int(11) DEFAULT NULL COMMENT '业务员',
  `waiter_id` int(11) DEFAULT NULL COMMENT '年检客服',
  `accountant_id` int(11) DEFAULT NULL COMMENT '会计',
  `manager_id` int(11) DEFAULT NULL COMMENT '经理',
  `organization_id` int(11) DEFAULT NULL COMMENT '业务员部门',
  `description` varchar(300) NULL,
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `customer_id` (`customer_id`),
  KEY `creator_id` (`creator_id`),
  KEY `salesman_id` (`salesman_id`),
  KEY `waiter_id` (`waiter_id`),
  KEY `accountant_id` (`accountant_id`),
  KEY `manager_id` (`manager_id`),
  KEY `finance_reviewer_id` (`finance_reviewer_id`),
  KEY `submit_reviewer_id` (`submit_reviewer_id`),
  CONSTRAINT `annual_ibfk_customer` FOREIGN KEY (`customer_id`) REFERENCES `customer` (`id`),
  CONSTRAINT `annual_ibfk_creator` FOREIGN KEY (`creator_id`) REFERENCES `member` (`id`),
  CONSTRAINT `annual_ibfk_salesman` FOREIGN KEY (`salesman_id`) REFERENCES `member` (`id`),
  CONSTRAINT `annual_ibfk_waiter` FOREIGN KEY (`waiter_id`) REFERENCES `member` (`id`),
  CONSTRAINT `annual_ibfk_accountant` FOREIGN KEY (`accountant_id`) REFERENCES `member` (`id`),
  CONSTRAINT `annual_ibfk_manager` FOREIGN KEY (`manager_id`) REFERENCES `member` (`id`),
  CONSTRAINT `annual_ibfk_finance_reviewer` FOREIGN KEY (`finance_reviewer_id`) REFERENCES `member` (`id`),
  CONSTRAINT `annual_ibfk_submit_reviewer` FOREIGN KEY (`submit_reviewer_id`) REFERENCES `member` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `settings`;
CREATE TABLE `settings` (
  `id` int(11) NOT NULL,
  `name` varchar(20) NULL,
  `value` text,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
INSERT INTO `settings` VALUES ('1', 'CODING', '{"customer":{"suffix":"4","area_code":[{"id":1,"name":"","value":"XM"},{"id":2,"name":"","value":"QZ"},{"id":3,"name":"","value":"QD"}]},"order":{"suffix":"4","code":[{"module":"ZW","module_name":"境外注册","value":"ZW"},{"module":"ZN","module_name":"境内注册","value":"ZN"},{"module":"SJ","module_name":"审计","value":"SJ"},{"module":"SB","module_name":"商标","value":"SB"},{"module":"ZL","module_name":"专利","value":"ZL"},{"module":"NS","module_name":"年审","value":"NS"},{"module":"NJ","module_name":"年检","value":"NJ"}]}}');


DROP TABLE IF EXISTS `menu`;
CREATE TABLE `menu` (
  `id` int(11) NOT NULL,
  `parent_id` int(11) NULL,
  `route` varchar(30) NULL,
  `icon` varchar(30) NULL,
  `name` varchar(50) NULL,
  `enable` tinyint(3) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
-- ----------------------------
-- Records of 客户管理
-- ----------------------------
INSERT INTO `menu` VALUES ('1', '0', '', 'fa fa-users', '客户管理', '1');
INSERT INTO `menu` VALUES ('2', '1', 'reserve', 'fa fa-user-md', '预备客户', '1');
INSERT INTO `menu` VALUES ('3', '1', 'customer', 'fa fa-user-secret', '正式客户', '1');
-- ----------------------------
-- Records of 订单管理
-- ----------------------------
INSERT INTO `menu` VALUES ('4', '0', '', 'fa fa-cart-arrow-down', '订单管理', '1');
INSERT INTO `menu` VALUES ('5', '4', 'abroad', 'fa fa-globe', '境外注册', '1');
INSERT INTO `menu` VALUES ('6', '4', 'internal', 'fa fa-street-view', '境内注册', '1');
INSERT INTO `menu` VALUES ('7', '4', 'trademark', 'fa fa-trademark', '商标订单', '1');
INSERT INTO `menu` VALUES ('8', '4', 'patent', 'fa fa-cube', '专利订单', '1');
INSERT INTO `menu` VALUES ('9', '4', 'audit', 'fa fa-calculator', '审计订单', '1');
INSERT INTO `menu` VALUES ('10', '4', 'annual_warning', 'fa fa-calculator', '年检预警', '1');
INSERT INTO `menu` VALUES ('11', '4', 'annual', 'fa fa-calculator', '年检列表', '1');
INSERT INTO `menu` VALUES ('12', '4', 'check_finance', 'fa fa-calculator', '订单财务审核', '1');
INSERT INTO `menu` VALUES ('13', '4', 'check_submit', 'fa fa-calculator', '订单提交审核', '1');
INSERT INTO `menu` VALUES ('14', '4', 'order_summary', 'fa fa-calculator', '订单汇总表', '1');
-- ----------------------------
-- Records of 快件登记管理
-- ----------------------------
INSERT INTO `menu` VALUES ('15', '0', '', 'fa fa-envelope-o', '快件登记管理', '1');
INSERT INTO `menu` VALUES ('16', '15', '', 'fa fa-list', '快件列表', '1');
-- ----------------------------
-- Records of 基本资料管理
-- ----------------------------
INSERT INTO `menu` VALUES ('17', '0', '', 'fa fa-university', '基本资料管理', '1');
INSERT INTO `menu` VALUES ('18', '17', 'organization', 'fa fa-sitemap', '组织架构', '1');
INSERT INTO `menu` VALUES ('19', '17', 'area', 'fa fa-crosshairs', '区域设置', '1');
INSERT INTO `menu` VALUES ('20', '17', 'position', 'fa fa-graduation-cap', '职位设置', '1');
INSERT INTO `menu` VALUES ('21', '17', 'member', 'fa fa-user', '用户管理', '1');
INSERT INTO `menu` VALUES ('22', '17', 'dictionary', 'fa fa-user', '数据字典', '1');
-- ----------------------------
-- Records of 权限管理
-- ----------------------------
INSERT INTO `menu` VALUES ('23', '0', '', 'fa fa-lock', '权限管理', '1');
INSERT INTO `menu` VALUES ('24', '23', 'role', 'fa fa-graduation-cap', '角色管理', '1');
INSERT INTO `menu` VALUES ('25', '23', 'role_assign', 'fa fa-key', '功能权限', '1');
INSERT INTO `menu` VALUES ('26', '23', 'role_user', 'fa fa-unlock-alt', '用户角色', '1');
-- ----------------------------
-- Records of 系统设置
-- ----------------------------
INSERT INTO `menu` VALUES ('27', '0', '', 'fa fa-cog', '系统设置', '1');
INSERT INTO `menu` VALUES ('28', '27', 'coding', 'fa fa-file-text-o', '编码规则', '1');


DROP TABLE IF EXISTS `role_member`;
CREATE TABLE `role_member` (
  `id` int(11) NOT NULL,
  `role_id` int(11) NULL,
  `member_id` int(11) NULL,
  PRIMARY KEY (`id`)
  -- KEY `role_id` (`role_id`),
  -- KEY `member_id` (`member_id`),
  -- CONSTRAINT `role_ibfk` FOREIGN KEY (`role_id`) REFERENCES `role` (`id`),
  -- CONSTRAINT `member_ibfk` FOREIGN KEY (`member_id`) REFERENCES `member` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `role_memu`;
CREATE TABLE `role_memu` (
  `id` int(11) NOT NULL,
  `role_id` int(11) NULL,
  `memu_id` int(11) NULL,
  PRIMARY KEY (`id`)
  -- KEY `role_id` (`role_id`),
  -- KEY `memu_id` (`memu_id`),
  -- CONSTRAINT `role_ibfk_menu` FOREIGN KEY (`role_id`) REFERENCES `role` (`id`),
  -- CONSTRAINT `memu_ibfk_role` FOREIGN KEY (`memu_id`) REFERENCES `menu` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `operation`;
CREATE TABLE `operation` (
  `id` int(11) NOT NULL,
  `name` varchar(20) NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

INSERT INTO `operation` VALUES ('1', '查看公司数据');
INSERT INTO `operation` VALUES ('2', '查看本部门数据');
INSERT INTO `operation` VALUES ('3', '财务审核');
INSERT INTO `operation` VALUES ('4', '提交审核');
INSERT INTO `operation` VALUES ('5', '年检权限');

DROP TABLE IF EXISTS `role_operation`;
CREATE TABLE `role_operation` (
  `id` int(11) NOT NULL,
  `role_id` int(11) NULL,
  `operation_id` int(11) NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `meetings`;
CREATE TABLE `meetings` (
  `id` int(11) NOT NULL,
  `type` varchar(50) DEFAULT NULL COMMENT '类型',
  `form` varchar(50) DEFAULT NULL COMMENT '形式',
  `title` varchar(100) DEFAULT NULL COMMENT '主题',
  `teacher` varchar(20) DEFAULT NULL COMMENT '讲师',3
  `sponsor` varchar(100) DEFAULT NULL COMMENT '主办方',
  `co_sponsor` varchar(300) DEFAULT NULL COMMENT '协办方',
  `customer_target` varchar(300) DEFAULT NULL COMMENT '客户群体',
  `date_meeting` datetime DEFAULT NULL COMMENT '时间',
  `form` varchar(50) DEFAULT NULL COMMENT '形式',
  `city` varchar(50) DEFAULT NULL COMMENT '城市',
  `address` varchar(200) DEFAULT NULL COMMENT '地址',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;







