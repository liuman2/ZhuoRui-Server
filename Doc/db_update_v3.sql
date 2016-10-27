DROP TABLE IF EXISTS `notice`;
CREATE TABLE `notice` (
  `id` int(11) NOT NULL,
  `creator_id` int(11) NULL,
  `title` varchar(50) DEFAULT NULL COMMENT '主题',
  `code` varchar(20) DEFAULT NULL COMMENT '编号',
  `type` int(11) NULL COMMENT '状态 1：公告 2：其他',
  `content` text,
  `status` int(11) NULL COMMENT '状态 0-待公布，1：已公布， 2：撤销',
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

INSERT INTO `sequence` VALUES ('notice', '0');

-- ----------------------------
-- Records of 公告管理
-- ----------------------------
INSERT INTO `menu` VALUES ('35', '0', '', 'fa fa-bullhorn', '公告管理', '8');
INSERT INTO `menu` VALUES ('36', '35', 'notice', 'fa fa-file-o', '通知公告', '1');
INSERT INTO `menu` VALUES ('37', '35', 'conference', 'fa fa-file-o', '会议纪要', '2');


