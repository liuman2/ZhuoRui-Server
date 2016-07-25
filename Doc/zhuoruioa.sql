
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
INSERT INTO `sequence` VALUES ('member', '1');

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
  `username` varchar(20) NOT NULL,
  `name` varchar(20) NOT NULL,
  `english_name` varchar(20) NOT NULL,
  `password` varchar(100) DEFAULT NULL,
  `mobile` varchar(20) DEFAULT NULL,
  `birthday` datetime DEFAULT NULL,
  `organization_id` int(11) DEFAULT NULL,
  `position_id` int(11) DEFAULT NULL,
  `role_id` int(11) DEFAULT NULL,
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `organization_id` (`organization_id`),
  KEY `position_id` (`position_id`),
  KEY `role_id` (`role_id`),
  CONSTRAINT `member_ibfk_1` FOREIGN KEY (`organization_id`) REFERENCES `organization` (`id`),
  CONSTRAINT `member_ibfk_2` FOREIGN KEY (`position_id`) REFERENCES `position` (`id`),
  CONSTRAINT `member_ibfk_3` FOREIGN KEY (`role_id`) REFERENCES `role` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='员工管理表';
