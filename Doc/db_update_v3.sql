DROP TABLE IF EXISTS `notice`;
CREATE TABLE `notice` (
  `id` int(11) NOT NULL,
  `creator_id` int(11) NULL,
  `title` varchar(50) DEFAULT NULL COMMENT '主题',
  `code` varchar(20) DEFAULT NULL COMMENT '编号',
  `content` text,
  `status` int(11) NULL COMMENT '状态 0-待公布，1：已公布， 2：撤销',
  `date_created` datetime NULL DEFAULT CURRENT_TIMESTAMP,
  `date_updated` datetime DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

INSERT INTO `sequence` VALUES ('notice', '0');


