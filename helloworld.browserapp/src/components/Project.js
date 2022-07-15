import React, { Component } from 'react';

import heart from '../images/heart.png';
import menuClosed from '../images/dots-vertical.png';
import menuOpened from '../images/dots-horizontal.png';
import DropDown from './DropDown';
import report from '../images/error.png';
import save from '../images/bookmark.png';
import rightArrow from '../images/right-arrow2.png';
import share from '../images/share.png';
import user from '../images/user.png';
import filledHeart from '../images/filled-heart.png';

import Tag from './Tag';
import ImageSlider from './ImageSlider';

import VisibilitySensor from 'react-visibility-sensor';

import { formatDate } from '../util';

import './Project.css';

class Project extends Component {
    state = {
        visibility: true
    }

    render() {
        return (
            <VisibilitySensor partialVisibility onChange={(isVisible) => isVisible && !this.props.creatorImage && this.props.onFirstAppear(this.props.keyProp)}>
                <div className="project-container" style={{
                        opacity: this.state.visibility ? 1 : 0.25,
                        transition: 'opacity 500ms linear',
                        width: this.props.width
                }} >
                    <div className="header-container">
                        <img src={this.props.creatorImage} alt="" height={30} width={30} />
                        <div className="header-middle">
                            <div className="header-text-container">
                                <p className="header-text">{this.props.title}</p>
                                {
                                    this.props.createdAt &&
                                    <p className="bold-gray-date header-date">{formatDate(this.props.createdAt)}</p>
                                }
                            </div>
                            <div className="header-tags">
                                {
                                    this.props.tags &&
                                    this.props.tags.map((item, index) =>
                                        <Tag key={index} name={item} />
                                    )
                                }
                            </div>
                        </div>
                        <span className="header-actions">
                            <div className="header-action">
                                <img src={user} width={30} height={30} alt="" />
                                <p className="header-likes">4</p>
                            </div>
                            <div className="header-action" style={{ marginRight: '0px' }}>
                                <img src={this.props.usersLikedIds?.findIndex(id => id === this.props.sessionUserId) !== -1 ? filledHeart : heart} width={30} height={30} alt=""
                                    onClick={() => this.props.onLike(this.props.keyProp)} />
                                <p className="header-likes">{this.props.usersLikedIds?.length ?? 0}</p>
                            </div>
                            <DropDown toggleButton={{
                                icon: undefined,
                                arrowIconOpen: menuOpened,
                                arrowIconClose: menuClosed
                            }}
                                arrowIconSize={30} onHeaderClick={this.handleMenu}>
                                <DropDown.Item icon={report} textColor="red" text="Report" iconSize={30} onClick={this.props.onReportClick} />
                                <DropDown.Item icon={rightArrow} text="Jump" iconSize={30} onClick={this.props.onRightArrowClick} />
                                <DropDown.Item icon={save} text="Save" iconSize={30} onClick={this.props.saveClick} />
                                <DropDown.Item icon={share} text="Share" iconSize={30} onClick={this.props.onShareClick} />
                            </DropDown>
                        </span>
                    </div>
                    <div className="description-container">
                        <p>{this.props.description}</p>
                    </div>
                    {
                        this.props.images !== undefined &&
                        <ImageSlider images={this.props.images} imageHeight={this.props.imageHeight} imageWidth={this.props.imageWidth} />
                    }
                </div>
            </VisibilitySensor>
        )
    }
}

export default Project;