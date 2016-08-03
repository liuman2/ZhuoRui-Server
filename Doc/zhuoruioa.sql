
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
INSERT INTO `sequence` VALUES ('dictionary_group', '7');
INSERT INTO `sequence` VALUES ('dictionary', '1');

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
INSERT INTO `role` (id, name, code, is_system, description) VALUES ('1', '系统管理员', 'admin',     '1',   '拥有最高权限');
INSERT INTO `role` (id, name, code, is_system, description) VALUES ('2', '总经理',     'manager',   '1',   '可查看公司所有业务资料');
INSERT INTO `role` (id, name, code, is_system, description) VALUES ('3', '部门经理',   'leader',    '1',   '可查看所属部门员工的客户资料和订单资料');
INSERT INTO `role` (id, name, code, is_system, description) VALUES ('4', '业务员',     'employee',  '1',   '只能查看自己的客户资料和订单资料');
INSERT INTO `role` (id, name, code, is_system, description) VALUES ('5', '财务',       'finance',   '1',   '财务审核权限');

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
INSERT INTO `dictionary_group` VALUES ('2', '客户来源',  '客户来源');
INSERT INTO `dictionary_group` VALUES ('3', '贸易方式', '贸易方式');
INSERT INTO `dictionary_group` VALUES ('4', '注册方式', '注册方式');
INSERT INTO `dictionary_group` VALUES ('5', '专利类型', '专利类型');
INSERT INTO `dictionary_group` VALUES ('6', '专利用途', '专利用途');

-- ----------------------------
-- Table structure for dictionary
-- ----------------------------
DROP TABLE IF EXISTS `dictionary`;
CREATE TABLE `dictionary` (
  `id` int(11) NOT NULL,
  `name` varchar(50) DEFAULT NULL,
  `group` varchar(20) DEFAULT NULL,
  `date_created` datetime DEFAULT NULL,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for customer
-- ----------------------------
DROP TABLE IF EXISTS `customer`;
CREATE TABLE `customer` (
  `id` int(11) NOT NULL,
  `name` varchar(50) DEFAULT NULL COMMENT '客户名称',
  `industry` varchar(50) DEFAULT NULL COMMENT '所属行业',
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
  `source` varchar(50) DEFAULT NULL,
  `creator_id` int(11) DEFAULT NULL COMMENT '创建者',
  `salesman_id` int(11) DEFAULT NULL COMMENT '业务员',
  `waiter_id` int(11) DEFAULT NULL COMMENT '年检客服',
  `manager_id` int(11) DEFAULT NULL COMMENT '经理',
  `outworker_id` int(11) DEFAULT NULL COMMENT '外勤',
  `organization_id` int(11) DEFAULT NULL COMMENT '业务员部门',
  `status` tinyint(3) DEFAULT NULL COMMENT '状态，0-预备，1-正式',
  `date_created` datetime DEFAULT NULL,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),

  KEY `creator_id` (`creator_id`),
  KEY `salesman_id` (`salesman_id`),
  KEY `waiter_id` (`waiter_id`),
  KEY `manager_id` (`manager_id`),
  KEY `outworker_id` (`outworker_id`),
  CONSTRAINT `member_ibfk_creator` FOREIGN KEY (`creator_id`) REFERENCES `member` (`id`),
  CONSTRAINT `member_ibfk_salesman` FOREIGN KEY (`salesman_id`) REFERENCES `member` (`id`),
  CONSTRAINT `member_ibfk_waiter` FOREIGN KEY (`waiter_id`) REFERENCES `member` (`id`),
  CONSTRAINT `member_ibfk_manager` FOREIGN KEY (`manager_id`) REFERENCES `member` (`id`),
  CONSTRAINT `member_ibfk_outworker` FOREIGN KEY (`outworker_id`) REFERENCES `member` (`id`)

) ENGINE=InnoDB DEFAULT CHARSET=utf8;


