DROP TABLE IF EXISTS `logtable`;
CREATE TABLE `logtable` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `created` datetime NOT NULL,
  `loglevel` int(11) NOT NULL,
  `groupname` varchar(100) NOT NULL DEFAULT '',
  `message` varchar(1024) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
);
