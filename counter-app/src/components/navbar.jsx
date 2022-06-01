import React from 'react';

const NavBar = ({ totalCounters }) => {
    return (
        <nav className="navbar bg-light" >
            <div className="container-fluid">
                <a className="navbar-brand" href="#">Items ({totalCounters})</a>
            </div>
        </nav>
    )
};

export default NavBar;

