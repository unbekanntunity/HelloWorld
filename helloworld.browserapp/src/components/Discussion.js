import React, { Component } from 'react';

import heart from '../images/heart.png';
import menuClosed from '../images/dots-vertical.png';
import menuOpened from '../images/dots-horizontal.png';
import DropDown from './DropDown';
import report from '../images/error.png';
import rightArrow from '../images/right-arrow2.png';
import share from '../images/share.png';
import filledHeart from '../images/filled-heart.png';
import remove from '../images/delete.png';
import save from '../images/bookmark.png';
import saved from '../images/bookmarked.png';

import Tag from './Tag';

import { formatDate } from '../util';
import VisibilitySensor from 'react-visibility-sensor';
    
import './Discussion.css';

class Discussion extends Component {
    state = {
        visibility: true
    }

    render() {
        let { title, tags, creatorImage, createdAt, creatorId, usersLikedIds, startMessage, previewMode, lastMessage } = this.props.item;

        return (
            <VisibilitySensor partialVisibility onChange={(isVisible) => isVisible && !creatorImage && this.props.onFirstAppear(this.props.keyProp)} >
            <div className="discussion-container" style={{
                opacity: this.state.visibility ? 1 : 0.25,
                transition: 'opacity 500ms linear',
                width: this.props.width
                }}>

                <div className="header-container">
                    <img src={this.props.creatorImage} alt="" height={30} width={30} />
                    <div className="header-middle">
                        <div className="header-text-container">
                            <p className="header-text">{title}</p>
                        </div>
                        <div className="header-tags">
                            {
                                tags &&
                                tags.map((item, index) =>
                                    <Tag key={index} name={item} />
                                )
                            }
                        </div>
                    </div>
                        <span className="header-actions">
                            <div className="header-action" onClick={() => this.props.onSave(this.props.id)}>
                                <img src={this.props.saved ? saved : save} alt="" height={30} width={30} />
                            </div>
                        <div className="header-action" style={{ marginRight: '0px' }}>
                            <img src={usersLikedIds?.findIndex(id => id === this.props.sessionUserId) !== -1 ? filledHeart : heart} width={30} height={30} alt=""
                                onClick={() => this.props.onLike(this.props.keyProp)} />
                            <p className="header-likes">{usersLikedIds?.length ?? 0}</p>
                            </div>
                            {
                                !previewMode &&
                                <DropDown toggleButton={{
                                    icon: undefined,
                                    arrowIconOpen: menuOpened,
                                    arrowIconClose: menuClosed
                                }} arrowIconSize={30} onHeaderClick={this.handleMenu}>
                                    {
                                        this.props.sessionUserId === creatorId &&
                                        <DropDown.Item icon={remove} textColor="red" text="Delete" iconSize={30} onClick={() => this.props.onDelete(this.props.keyProp)} />
                                    }
                                    <DropDown.Item icon={report} textColor="red" text="Report" iconSize={30} onClick={this.props.onReportClick} />
                                    <DropDown.Item icon={rightArrow} text="Jump" iconSize={30} onClick={this.props.onRightArrowClick} />
                                    <DropDown.Item icon={share} text="Share" iconSize={30} onClick={this.props.onShareClick} />
                                </DropDown>
                            }
                    </span>
                </div>
              
                <div className="description-container">
                    <p>{startMessage}</p>
                    <p className="discussion-description-date">{formatDate(createdAt)}</p>
                </div>
                {
                    this.props.lastMessage &&
                    <div className="discussion-lastMessage-container">
                        <div className="discussion-lastMessage">
                            <b>{lastMessage.creatorName}</b>
                            <p>: {lastMessage.content}</p>
                        </div>
                        <p className="discussion-lastMessage-date">{formatDate(lastMessage.createdAt)}</p>
                    </div>
                }
                </div>
            </VisibilitySensor>
        )
    }
}

export default Discussion;